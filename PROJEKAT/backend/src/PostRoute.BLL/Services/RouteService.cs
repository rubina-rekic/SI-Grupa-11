using PostRoute.BLL.Models.Routes;
using PostRoute.DAL.Entities;
using PostRoute.DAL.Repositories;

namespace PostRoute.BLL.Services;

public class RouteService : IRouteService
{
    private readonly IMailboxRepository _mailboxRepository;
    private readonly IRouteRepository _routeRepository;
    
    // MVP configurable defaults
    private const decimal StartingLat = 43.8563m; // Primjer centra Sarajeva (Glavni depo)
    private const decimal StartingLng = 18.4131m;
    private const int SpeedKmh = 30; // Prosjek brzine u gradu
    private const int StopDurationMinutes = 5; // Vrijeme provedeno na svakom sanducicu
    private const int MaxPoints = 50; // Maksimalno stavki rute

    public RouteService(IMailboxRepository mailboxRepository, IRouteRepository routeRepository)
    {
        _mailboxRepository = mailboxRepository;
        _routeRepository = routeRepository;
    }

    public async Task<RouteResponse> GenerateRouteAsync(GenerateRouteRequest request, CancellationToken cancellationToken = default)
    {
        var mailboxes = await _mailboxRepository.GetAllAsync(cancellationToken);
        
        // 1. Filtriranje po aktivnosti
        var activeMailboxes = mailboxes.Where(m => m.IsActive).ToList();

        // 2. Filtriranje po radnom danu
        var targetDayFlag = MapDayOfWeek(request.Date.DayOfWeek);
        var todayMailboxes = activeMailboxes.Where(m => m.WorkingDays == MailboxWorkingDays.None || (m.WorkingDays & targetDayFlag) == targetDayFlag).ToList();

        var unvisited = todayMailboxes.ToList();
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
            Date = route.Date,
            PlannedStartTime = route.PlannedStartTime,
            PlannedEndTime = route.PlannedEndTime,
            TotalDistanceKm = Math.Round(route.TotalDistanceKm, 2),
            TotalDurationMinutes = route.TotalDurationMinutes,
            Status = route.Status.ToString(),
            ExceedsStandardTime = route.ExceedsStandardTime,
            TotalMailboxesCount = mailboxes.Count(),
            ActiveMailboxesCount = activeMailboxes.Count,
            DayFilteredMailboxesCount = todayMailboxes.Count,
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
                Status = ri.Status
            }).ToList()
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

    private MailboxWorkingDays MapDayOfWeek(DayOfWeek dayOfWeek)
    {
        return dayOfWeek switch
        {
            DayOfWeek.Monday => MailboxWorkingDays.Ponedjeljak,
            DayOfWeek.Tuesday => MailboxWorkingDays.Utorak,
            DayOfWeek.Wednesday => MailboxWorkingDays.Srijeda,
            DayOfWeek.Thursday => MailboxWorkingDays.Cetvrtak,
            DayOfWeek.Friday => MailboxWorkingDays.Petak,
            DayOfWeek.Saturday => MailboxWorkingDays.Subota,
            DayOfWeek.Sunday => MailboxWorkingDays.Nedjelja,
            _ => MailboxWorkingDays.None
        };
    }
}
