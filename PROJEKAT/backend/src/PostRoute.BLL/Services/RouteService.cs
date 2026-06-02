using PostRoute.BLL.Models;
using PostRoute.BLL.Models.Routes;
using PostRoute.DAL.Entities;
using PostRoute.DAL.Repositories;
using PostRoute.Domain.Entities;

namespace PostRoute.BLL.Services;

public class RouteService : IRouteService
{
    private readonly IMailboxRepository _mailboxRepository;
    private readonly IRouteRepository _routeRepository;
    private readonly IUserRepository? _userRepository;
    
    // MVP configurable defaults
    private const decimal StartingLat = 43.8563m; // Primjer centra Sarajeva (Glavni depo)
    private const decimal StartingLng = 18.4131m;
    private const int SpeedKmh = 30; // Prosjek brzine u gradu
    private const int StopDurationMinutes = 5; // Vrijeme provedeno na svakom sanducicu
    private const int MaxPoints = 50; // Maksimalno stavki rute
    private const int MediumPriorityCooldownDays = 2;
    private const int LowPriorityCooldownDays = 4;

    public RouteService(
        IMailboxRepository mailboxRepository,
        IRouteRepository routeRepository,
        IUserRepository? userRepository = null)
    {
        _mailboxRepository = mailboxRepository;
        _routeRepository = routeRepository;
        _userRepository = userRepository;
    }

    public async Task<RouteResponse?> GetRouteDetailsAsync(Guid routeId, CancellationToken cancellationToken = default)
    {
        var route = await _routeRepository.GetByIdAsync(routeId, cancellationToken);
        if (route == null)
        {
            return null;
        }

        await NormalizeRouteExecutionStateAsync(route, cancellationToken);
        return MapToResponse(route, totalMailboxesCount: null, activeMailboxesCount: null, eligibleMailboxesCount: null);
    }

    public async Task<RouteResponse?> GetPostmanAssignedRouteForTodayAsync(Guid postmanId, CancellationToken cancellationToken = default)
    {
        var today = DateOnly.FromDateTime(DateTime.Now);
        var route = await _routeRepository.GetByPostmanAndDateAsync(postmanId, today, cancellationToken);
        
        if (route == null)
        {
            return null;
        }

        // Verify the route is in an accessible state for postman (assigned or in progress)
        if (route.Status != RouteStatus.Dodijeljena && route.Status != RouteStatus.UProgresu)
        {
            return null;
        }

        await NormalizeRouteExecutionStateAsync(route, cancellationToken);
        return MapToResponse(route, totalMailboxesCount: null, activeMailboxesCount: null, eligibleMailboxesCount: null);
    }

    public async Task<IReadOnlyList<AvailablePostmanResponse>> GetAvailablePostmenAsync(
        Guid routeId,
        CancellationToken cancellationToken = default)
    {
        var userRepository = _userRepository
            ?? throw new InvalidOperationException("Korisnicki repozitorij nije dostupan.");

        var route = await _routeRepository.GetByIdAsync(routeId, cancellationToken)
            ?? throw new InvalidOperationException("Ruta nije pronadjena.");

        var users = await userRepository.GetAllAsync(cancellationToken);
        var occupiedPostmanIds = await _routeRepository.GetPostmanIdsWithActiveRouteOnDateAsync(
            route.Date,
            route.Id,
            cancellationToken);
        var occupiedSet = occupiedPostmanIds.ToHashSet();

        return users
            .Where(user => user.Role == UserRole.PostalWorker && !user.IsLockedOut)
            .Select(user =>
            {
                var isCurrentAssignee = route.PostmanId == user.Id && route.Status == RouteStatus.Dodijeljena;
                var isBusy = occupiedSet.Contains(user.Id);

                return new AvailablePostmanResponse
                {
                    Id = user.Id,
                    FullName = ToDisplayName(user),
                    Username = user.Username,
                    Email = user.Email,
                    IsCurrentAssignee = isCurrentAssignee,
                    IsAvailable = isCurrentAssignee || !isBusy,
                    UnavailableReason = isBusy && !isCurrentAssignee
                        ? "Postar vec ima dodijeljenu rutu za ovaj datum."
                        : null
                };
            })
            .OrderBy(user => user.FullName)
            .ToList();
    }

    public async Task<RouteResponse> AssignRouteAsync(
        Guid routeId,
        AssignRouteRequest request,
        string assignedBy,
        CancellationToken cancellationToken = default)
    {
        var userRepository = _userRepository
            ?? throw new InvalidOperationException("Korisnicki repozitorij nije dostupan.");

        if (request.PostmanId == Guid.Empty)
        {
            throw new InvalidOperationException("Postar je obavezan.");
        }

        var route = await _routeRepository.GetByIdAsync(routeId, cancellationToken)
            ?? throw new InvalidOperationException("Ruta nije pronadjena.");

        if (route.Status != RouteStatus.Planirana && route.Status != RouteStatus.Dodijeljena)
        {
            throw new InvalidOperationException("Dodjela rute je dostupna samo za prijedloge ili vec dodijeljene rute.");
        }

        var postman = await userRepository.GetByIdAsync(request.PostmanId, cancellationToken)
            ?? throw new InvalidOperationException("Postar nije pronadjen.");

        if (postman.Role != UserRole.PostalWorker || postman.IsLockedOut)
        {
            throw new InvalidOperationException("Odabrani korisnik nije aktivan postar.");
        }

        var occupiedPostmanIds = await _routeRepository.GetPostmanIdsWithActiveRouteOnDateAsync(
            route.Date,
            route.Id,
            cancellationToken);

        if (occupiedPostmanIds.Contains(postman.Id))
        {
            throw new InvalidOperationException("Postar vec ima dodijeljenu rutu za ovaj datum.");
        }

        route.PostmanId = postman.Id;
        route.Postman = postman;
        route.Status = RouteStatus.Dodijeljena;
        route.AssignedAt = DateTime.UtcNow;
        route.AssignedBy = assignedBy;

        await _routeRepository.UpdateAsync(route, cancellationToken);

        return MapToResponse(route, null, null, null);
    }

    public async Task<RouteResponse> GenerateRouteAsync(GenerateRouteRequest request, CancellationToken cancellationToken = default)
    {
        var existingRoute = await _routeRepository.GetByPostmanAndDateAsync(request.PostmanId, request.Date, cancellationToken);
        if (existingRoute is not null)
        {
            return MapToResponse(existingRoute, totalMailboxesCount: null, activeMailboxesCount: null, eligibleMailboxesCount: null);
        }

        var mailboxes = await _mailboxRepository.GetAllAsync(cancellationToken);
        
        // 1. Filtriranje po aktivnosti
        var activeMailboxes = mailboxes.Where(m => m.IsActive).ToList();
        var lastIncludedByMailbox = await _routeRepository.GetLastIncludedDatesByMailboxIdsAsync(
            activeMailboxes.Select(x => x.Id),
            request.Date,
            cancellationToken);
        
        // 2. Filtriranje po radnim danima i prioritetnim pravilima
        var routeDayFlag = ToDayFlag(request.Date);
        var eligibleMailboxes = activeMailboxes
            .Where(mailbox => (mailbox.WorkingDays & routeDayFlag) != 0)
            .Where(mailbox => IsEligibleByPriority(mailbox, request.Date, lastIncludedByMailbox))
            .ToList();

        var unvisited = eligibleMailboxes.ToList();
        var routeItems = new List<RouteItem>();
        
        decimal currentLat = StartingLat;
        decimal currentLng = StartingLng;
        int order = 1;
        TimeOnly currentTime = request.PlannedStartTime;
        decimal totalDistance = 0;

        // Iteriraj od najvišeg (1) do najnižeg (3) prioriteta
        var priorities = new[] { MailboxPriority.Visok, MailboxPriority.Srednji, MailboxPriority.Nizak };

        foreach (var prio in priorities)
        {
            var prioUnvisited = unvisited.Where(u => u.Priority == prio).ToList();

            while (prioUnvisited.Count > 0 && order <= MaxPoints)
            {
                Mailbox? bestMatch = null;
                decimal bestDistance = decimal.MaxValue;
                TimeOnly bestArrivalTime = currentTime;

                foreach (var m in prioUnvisited)
                {
                    var distance = CalculateEuclideanDistance(currentLat, currentLng, m.Latitude, m.Longitude);
                    int travelMinutes = CalculateTravelMinutes(distance);
                    var arrivalTime = currentTime.AddMinutes(travelMinutes);

                    if (IsAvailableAt(m, arrivalTime))
                    {
                        if (distance < bestDistance)
                        {
                            bestDistance = distance;
                            bestMatch = m;
                            bestArrivalTime = arrivalTime;
                        }
                    }
                }

                if (bestMatch != null)
                {
                    prioUnvisited.Remove(bestMatch);
                    unvisited.Remove(bestMatch);
                    
                    totalDistance += bestDistance;
                    currentTime = bestArrivalTime;

                    routeItems.Add(new RouteItem
                    {
                        MailboxId = bestMatch.Id,
                        Mailbox = bestMatch, // temp for response mapping
                        Order = order++,
                        EstimatedArrivalTime = currentTime,
                        Status = "Planirano"
                    });
                    
                    currentTime = currentTime.AddMinutes(StopDurationMinutes);
                    currentLat = bestMatch.Latitude;
                    currentLng = bestMatch.Longitude;
                }
                else
                {
                    // Nema dostupnih sandučića za posjetu u trenutnom vremenu unutar ovog prioriteta
                    // Izbacujemo ih iz liste kako ne bi vrtili u nedogled
                    prioUnvisited.Clear();
                }
            }
            if (order > MaxPoints) break;
        }

        var totalDuration = (int)(currentTime - request.PlannedStartTime).TotalMinutes;
        var route = new Route
        {
            PostmanId = request.PostmanId,
            Date = request.Date,
            PlannedStartTime = request.PlannedStartTime,
            PlannedEndTime = currentTime,
            Status = RouteStatus.Planirana,
            TotalDistanceKm = totalDistance * 111m, // pretvaranje euklidske u približnu udaljenost
            TotalDurationMinutes = totalDuration,
            ExceedsStandardTime = totalDuration > 480,
            RouteItems = routeItems.Select(ri => new RouteItem
            {
                MailboxId = ri.MailboxId,
                Order = ri.Order,
                EstimatedArrivalTime = ri.EstimatedArrivalTime,
                Status = ri.Status
            }).ToList()
        };

        if (route.RouteItems.Count > 0)
        {
            await _routeRepository.CreateAsync(route, cancellationToken);
        }

        return new RouteResponse
        {
            Id = route.Id,
            PostmanId = route.PostmanId,
            PostmanName = route.Postman is null ? null : ToDisplayName(route.Postman),
            Date = route.Date,
            PlannedStartTime = route.PlannedStartTime,
            PlannedEndTime = route.PlannedEndTime,
            TotalDistanceKm = Math.Round(route.TotalDistanceKm, 2),
            TotalDurationMinutes = route.TotalDurationMinutes,
            Status = route.Status.ToString(),
            ExceedsStandardTime = route.ExceedsStandardTime,
            AssignedAt = route.AssignedAt,
            AssignedBy = route.AssignedBy,
            StartedAt = route.StartedAt,
            CompletedAt = route.CompletedAt,
            TotalMailboxesCount = mailboxes.Count(),
            ActiveMailboxesCount = activeMailboxes.Count,
            DayFilteredMailboxesCount = eligibleMailboxes.Count,
            RouteItems = routeItems.Select(ri => new RouteItemResponse
            {
                Id = ri.Id,
                MailboxId = ri.MailboxId,
                Address = ri.Mailbox.Address,
                Latitude = ri.Mailbox.Latitude,
                Longitude = ri.Mailbox.Longitude,
                Order = ri.Order,
                EstimatedArrivalTime = ri.EstimatedArrivalTime,
                Priority = ri.Mailbox.Priority.ToString(),
                Status = ri.Status,
                IsManuallyReordered = false,
                MailboxStatus = ri.ProcessedStatus?.ToString() ?? ri.Mailbox.Status.ToString(),
                ProcessedAt = ri.ProcessedAt,
                ProcessedBy = ri.ProcessedBy,
                ProcessedStatus = ri.ProcessedStatus?.ToString(),
                UnavailableReason = ri.UnavailableReason
            }).ToList()
        };
    }

    public async Task<RouteResponse> ReorderRouteAsync(Guid routeId, ReorderRouteRequest request, string reorderedBy, CancellationToken cancellationToken = default)
    {
        var route = await _routeRepository.GetByIdAsync(routeId, cancellationToken)
            ?? throw new InvalidOperationException("Ruta nije pronađena.");

        if (route.Status == RouteStatus.UProgresu || route.Status == RouteStatus.Zavrsena)
            throw new InvalidOperationException("Izmjena redoslijeda nije dostupna za rute u toku ili završene rute.");

        var orderMap = request.Items.ToDictionary(i => i.RouteItemId, i => i.NewOrder);

        // Skup ID-ova koji su zaista premješteni (imaju drugačiji order nego što je bio)
        var originalOrderMap = route.RouteItems.ToDictionary(ri => ri.Id, ri => ri.Order);

        foreach (var item in route.RouteItems)
        {
            if (orderMap.TryGetValue(item.Id, out var newOrder))
            {
                item.IsManuallyReordered = originalOrderMap[item.Id] != newOrder;
                item.Order = newOrder;
            }
        }

        // Ponovna kalkulacija EstimatedArrivalTime po novom redoslijedu
        var sorted = route.RouteItems.OrderBy(ri => ri.Order).ToList();
        decimal currentLat = StartingLat;
        decimal currentLng = StartingLng;
        TimeOnly currentTime = route.PlannedStartTime;

        foreach (var item in sorted)
        {
            var distance = CalculateEuclideanDistance(currentLat, currentLng, item.Mailbox.Latitude, item.Mailbox.Longitude);
            int travelMinutes = CalculateTravelMinutes(distance);
            currentTime = currentTime.AddMinutes(travelMinutes);
            item.EstimatedArrivalTime = currentTime;
            currentTime = currentTime.AddMinutes(StopDurationMinutes);
            currentLat = item.Mailbox.Latitude;
            currentLng = item.Mailbox.Longitude;
        }

        var totalDuration = (int)(currentTime - route.PlannedStartTime).TotalMinutes;
        route.PlannedEndTime = currentTime;
        route.TotalDurationMinutes = totalDuration;
        route.ExceedsStandardTime = totalDuration > 480;
        route.LastReorderedAt = DateTime.UtcNow;
        route.LastReorderedBy = reorderedBy;

        await _routeRepository.UpdateAsync(route, cancellationToken);

        return MapToResponse(route, null, null, null);
    }

    private bool IsEligibleByPriority(
        Mailbox mailbox,
        DateOnly routeDate,
        IReadOnlyDictionary<Guid, DateOnly> lastIncludedByMailbox)
    {
        if (!lastIncludedByMailbox.TryGetValue(mailbox.Id, out var lastIncludedDate))
        {
            return true;
        }

        if (lastIncludedDate == routeDate)
        {
            return false;
        }

        var daysSinceLastInclude = routeDate.DayNumber - lastIncludedDate.DayNumber;

        return mailbox.Priority switch
        {
            MailboxPriority.Visok => true,
            MailboxPriority.Srednji => daysSinceLastInclude >= MediumPriorityCooldownDays,
            MailboxPriority.Nizak => daysSinceLastInclude >= LowPriorityCooldownDays,
            _ => true
        };
    }

    private decimal CalculateEuclideanDistance(decimal lat1, decimal lon1, decimal lat2, decimal lon2)
    {
        return (decimal)Math.Sqrt((double)((lat2 - lat1) * (lat2 - lat1) + (lon2 - lon1) * (lon2 - lon1)));
    }

    private int CalculateTravelMinutes(decimal distanceInDegrees)
    {
        var distanceKm = distanceInDegrees * 111m;
        return (int)Math.Round((distanceKm / SpeedKmh) * 60m);
    }

    private static MailboxWorkingDays ToDayFlag(DateOnly date) => date.DayOfWeek switch
    {
        DayOfWeek.Monday    => MailboxWorkingDays.Ponedjeljak,
        DayOfWeek.Tuesday   => MailboxWorkingDays.Utorak,
        DayOfWeek.Wednesday => MailboxWorkingDays.Srijeda,
        DayOfWeek.Thursday  => MailboxWorkingDays.Cetvrtak,
        DayOfWeek.Friday    => MailboxWorkingDays.Petak,
        DayOfWeek.Saturday  => MailboxWorkingDays.Subota,
        _                   => MailboxWorkingDays.Nedjelja,
    };

    private bool IsAvailableAt(Mailbox m, TimeOnly arrival)
    {
        if (m.IsAlwaysAvailable) return true;
        
        // Ako nema postavljenih slotova (null), pretpostavljamo da nema restrikcija (zaboravljen unos)
        if (!m.Slot1Start.HasValue && !m.Slot2Start.HasValue) return true;
        
        bool inSlot1 = m.Slot1Start.HasValue && m.Slot1End.HasValue && 
                       arrival >= m.Slot1Start.Value && arrival <= m.Slot1End.Value;
                       
        bool inSlot2 = m.Slot2Start.HasValue && m.Slot2End.HasValue && 
                       arrival >= m.Slot2Start.Value && arrival <= m.Slot2End.Value;
                       
        return inSlot1 || inSlot2;
    }

    public async Task<List<RouteResponse>> GetRoutesForDateAsync(DateOnly date, CancellationToken cancellationToken = default)
    {
        var routes = await _routeRepository.GetByDateAsync(date, cancellationToken);
        foreach (var route in routes)
        {
            await NormalizeRouteExecutionStateAsync(route, cancellationToken);
        }

        return routes
            .Select(r => MapToResponse(r, null, null, null))
            .ToList();
    }

    private async Task NormalizeRouteExecutionStateAsync(Route route, CancellationToken cancellationToken)
    {
        if (route.RouteItems.Count == 0 || route.Status == RouteStatus.Otkazana)
        {
            return;
        }

        var processedItems = route.RouteItems
            .Where(IsRouteItemProcessed)
            .ToList();

        if (processedItems.Count == 0)
        {
            return;
        }

        var now = DateTime.UtcNow;
        var changed = false;
        var firstProcessedAt = processedItems
            .Where(item => item.ProcessedAt.HasValue)
            .Select(item => item.ProcessedAt!.Value)
            .DefaultIfEmpty(now)
            .Min();

        if (route.StartedAt is null)
        {
            route.StartedAt = firstProcessedAt;
            changed = true;
        }

        if (processedItems.Count == route.RouteItems.Count)
        {
            var completedAt = processedItems
                .Where(item => item.ProcessedAt.HasValue)
                .Select(item => item.ProcessedAt!.Value)
                .DefaultIfEmpty(now)
                .Max();

            if (route.Status != RouteStatus.Zavrsena)
            {
                route.Status = RouteStatus.Zavrsena;
                changed = true;
            }

            if (route.CompletedAt is null)
            {
                route.CompletedAt = completedAt;
                changed = true;
            }
        }
        else if (route.Status == RouteStatus.Planirana || route.Status == RouteStatus.Dodijeljena)
        {
            route.Status = RouteStatus.UProgresu;
            changed = true;
        }

        if (changed)
        {
            await _routeRepository.UpdateAsync(route, cancellationToken);
        }
    }

    private static bool IsRouteItemProcessed(RouteItem item)
    {
        if (item.ProcessedAt.HasValue || item.ProcessedStatus.HasValue)
        {
            return true;
        }

        var status = (item.Status ?? string.Empty).Trim();
        return status.Equals("Obrađen", StringComparison.OrdinalIgnoreCase) ||
               status.Equals("Obradjen", StringComparison.OrdinalIgnoreCase) ||
               status.Equals("Obradeno", StringComparison.OrdinalIgnoreCase) ||
               status.Equals("Obraen", StringComparison.OrdinalIgnoreCase) ||
               status.Equals("Nedostupan", StringComparison.OrdinalIgnoreCase);
    }

    public async Task<PagedResult<RouteResponse>> GetArchiveAsync(
        int page,
        int pageSize,
        DateOnly? fromDate,
        DateOnly? toDate,
        Guid? postmanId,
        CancellationToken cancellationToken = default)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 25;
        if (pageSize > 100) pageSize = 100;

        var (items, total) = await _routeRepository.GetPagedArchiveAsync(
            page, pageSize, fromDate, toDate, postmanId, cancellationToken);

        var list = items.Select(r => MapToResponse(r, null, null, null)).ToList();
        return new PagedResult<RouteResponse>(list, total, page, pageSize);
    }

    public async Task<PostmanPerformanceReportResponse> GetPostmanPerformanceReportAsync(
        DateOnly fromDate,
        DateOnly toDate,
        CancellationToken cancellationToken = default)
    {
        if (fromDate > toDate)
        {
            throw new InvalidOperationException("Pocetni datum ne moze biti poslije zavrsnog datuma.");
        }

        var routes = await _routeRepository.GetCompletedRoutesForPerformanceReportAsync(
            fromDate,
            toDate,
            cancellationToken);

        var rows = routes
            .GroupBy(route => route.PostmanId)
            .Select(group =>
            {
                var firstRoute = group.First();
                var routeRows = group
                    .OrderByDescending(route => route.Date)
                    .ThenByDescending(route => route.PlannedStartTime)
                    .Select(MapPerformanceRoute)
                    .ToList();

                var assigned = routeRows.Sum(route => route.AssignedMailboxes);
                var emptied = routeRows.Sum(route => route.EmptiedLocations);
                var unrealized = assigned - emptied;

                return new PostmanPerformanceRowResponse
                {
                    PostmanId = group.Key,
                    PostmanName = firstRoute.Postman is null ? group.Key.ToString() : ToDisplayName(firstRoute.Postman),
                    AssignedMailboxes = assigned,
                    EmptiedLocations = emptied,
                    UnrealizedLocations = unrealized,
                    SuccessPercentage = CalculateSuccessPercentage(emptied, assigned),
                    CompletedRoutesCount = routeRows.Count,
                    Routes = routeRows
                };
            })
            .OrderByDescending(row => row.SuccessPercentage)
            .ThenBy(row => row.PostmanName)
            .ToList();

        return new PostmanPerformanceReportResponse
        {
            FromDate = fromDate,
            ToDate = toDate,
            TotalPostmen = rows.Count,
            TotalAssignedMailboxes = rows.Sum(row => row.AssignedMailboxes),
            TotalEmptiedLocations = rows.Sum(row => row.EmptiedLocations),
            TotalUnrealizedLocations = rows.Sum(row => row.UnrealizedLocations),
            TeamAverageSuccessPercentage = rows.Count == 0
                ? 0
                : Math.Round(rows.Average(row => row.SuccessPercentage), 2),
            Rows = rows
        };
    }

    public async Task<MailboxTypeRealizationReportResponse> GetMailboxTypeRealizationReportAsync(
        DateOnly fromDate,
        DateOnly toDate,
        CancellationToken cancellationToken = default)
    {
        if (fromDate > toDate)
        {
            throw new InvalidOperationException("Pocetni datum ne moze biti poslije zavrsnog datuma.");
        }

        var routes = await _routeRepository.GetCompletedRoutesForPerformanceReportAsync(
            fromDate,
            toDate,
            cancellationToken);

        var typeGroups = routes
            .SelectMany(route => route.RouteItems, (route, item) => new { route, item })
            .GroupBy(x => x.item.Mailbox.Type)
            .Select(group =>
            {
                var typeId = (int)group.Key;
                var planned = group.Count();
                var successful = group.Count(x => IsSuccessfullyEmptied(x.item));
                var problems = group.Count(x => IsRouteItemProblem(x.item));

                var details = group
                    .Where(x => IsRouteItemProblem(x.item))
                    .OrderBy(x => x.route.Date)
                    .ThenBy(x => x.item.Mailbox.Address)
                    .Select(x => new MailboxTypeRealizationDetailResponse
                    {
                        MailboxId = x.item.MailboxId,
                        Address = x.item.Mailbox.Address,
                        RouteDate = x.route.Date,
                        Status = x.item.ProcessedStatus?.ToString() ?? x.item.Status,
                        Notes = x.item.UnavailableReason
                    })
                    .ToList();

                return new MailboxTypeRealizationRowResponse
                {
                    TypeId = typeId,
                    TypeName = group.Key.ToString(),
                    PlannedEmpties = planned,
                    SuccessfulEmpties = successful,
                    ProblemReports = problems,
                    FailureRate = CalculateSuccessPercentage(problems, planned),
                    Details = details
                };
            })
            .OrderByDescending(row => row.FailureRate)
            .ThenBy(row => row.TypeName)
            .ToList();

        var totalPlanned = typeGroups.Sum(row => row.PlannedEmpties);
        var totalSuccessful = typeGroups.Sum(row => row.SuccessfulEmpties);
        var totalProblems = typeGroups.Sum(row => row.ProblemReports);

        return new MailboxTypeRealizationReportResponse
        {
            FromDate = fromDate,
            ToDate = toDate,
            TotalTypes = typeGroups.Count,
            TotalPlannedEmpties = totalPlanned,
            TotalSuccessfulEmpties = totalSuccessful,
            TotalProblemReports = totalProblems,
            AverageFailureRate = totalPlanned == 0 ? 0 : Math.Round((decimal)totalProblems / totalPlanned * 100m, 2),
            Rows = typeGroups
        };
    }

    private static bool IsRouteItemProblem(RouteItem item)
    {
        if (item.ProcessedStatus == MailboxStatus.Nedostupan)
        {
            return true;
        }

        if (string.Equals(item.Status, nameof(MailboxStatus.Nedostupan), StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return !string.IsNullOrWhiteSpace(item.UnavailableReason);
    }

    private static PostmanPerformanceRouteResponse MapPerformanceRoute(Route route)
    {
        var assigned = route.RouteItems.Count;
        var emptied = route.RouteItems.Count(IsSuccessfullyEmptied);
        var unrealized = assigned - emptied;

        return new PostmanPerformanceRouteResponse
        {
            RouteId = route.Id,
            Date = route.Date,
            PlannedStartTime = route.PlannedStartTime,
            CompletedAt = route.CompletedAt,
            AssignedMailboxes = assigned,
            EmptiedLocations = emptied,
            UnrealizedLocations = unrealized,
            SuccessPercentage = CalculateSuccessPercentage(emptied, assigned)
        };
    }

    private static bool IsSuccessfullyEmptied(RouteItem item)
    {
        if (item.ProcessedStatus == MailboxStatus.Ispraznjen)
        {
            return true;
        }

        return string.Equals(item.Status, nameof(MailboxStatus.Ispraznjen), StringComparison.OrdinalIgnoreCase);
    }

    private static decimal CalculateSuccessPercentage(int emptied, int assigned)
    {
        if (assigned == 0)
        {
            return 0;
        }

        return Math.Round((decimal)emptied / assigned * 100m, 2);
    }

    private static string ToDisplayName(User user)
    {
        var fullName = $"{user.FirstName} {user.LastName}".Trim();
        return string.IsNullOrWhiteSpace(fullName) ? user.Username : fullName;
    }

    private static RouteResponse MapToResponse(Route route, int? totalMailboxesCount, int? activeMailboxesCount, int? eligibleMailboxesCount)
    {
        var orderedItems = route.RouteItems
            .OrderBy(ri => ri.Order)
            .ToList();

        return new RouteResponse
        {
            Id = route.Id,
            PostmanId = route.PostmanId,
            PostmanName = route.Postman is null ? null : ToDisplayName(route.Postman),
            Date = route.Date,
            PlannedStartTime = route.PlannedStartTime,
            PlannedEndTime = route.PlannedEndTime,
            TotalDistanceKm = Math.Round(route.TotalDistanceKm, 2),
            TotalDurationMinutes = route.TotalDurationMinutes,
            Status = route.Status.ToString(),
            ExceedsStandardTime = route.ExceedsStandardTime,
            LastReorderedAt = route.LastReorderedAt,
            LastReorderedBy = route.LastReorderedBy,
            AssignedAt = route.AssignedAt,
            AssignedBy = route.AssignedBy,
            StartedAt = route.StartedAt,
            CompletedAt = route.CompletedAt,
            TotalMailboxesCount = totalMailboxesCount ?? 0,
            ActiveMailboxesCount = activeMailboxesCount ?? 0,
            DayFilteredMailboxesCount = eligibleMailboxesCount ?? orderedItems.Count,
            RouteItems = orderedItems.Select(ri => new RouteItemResponse
            {
                Id = ri.Id,
                MailboxId = ri.MailboxId,
                Address = ri.Mailbox.Address,
                Latitude = ri.Mailbox.Latitude,
                Longitude = ri.Mailbox.Longitude,
                Order = ri.Order,
                EstimatedArrivalTime = ri.EstimatedArrivalTime,
                Priority = ri.Mailbox.Priority.ToString(),
                Status = ri.Status,
                IsManuallyReordered = ri.IsManuallyReordered,
                MailboxStatus = ri.ProcessedStatus?.ToString() ?? ri.Mailbox.Status.ToString(),
                ProcessedAt = ri.ProcessedAt,
                ProcessedBy = ri.ProcessedBy,
                ProcessedStatus = ri.ProcessedStatus?.ToString(),
                UnavailableReason = ri.UnavailableReason
            }).ToList()
        };
    }
}
