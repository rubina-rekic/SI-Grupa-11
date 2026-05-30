import { fireEvent, render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { BrowserRouter, MemoryRouter, Route, Routes } from 'react-router-dom'
import { beforeEach, describe, expect, it, vi } from 'vitest'
import ArchiveRouteListPage from '../ArchiveRouteListPage'
import ArchiveRouteDetailsPage from '../ArchiveRouteDetailsPage'
import type { RouteResponse } from '../../../../infrastructure/api/routesApi'

vi.mock('../../../components/Layout/Layout', () => ({
  Layout: ({ children }: { children: React.ReactNode }) => <div>{children}</div>,
}))

vi.mock('react-leaflet', () => ({
  MapContainer: ({ children }: { children: React.ReactNode }) => <div data-testid="archive-map">{children}</div>,
  Marker: ({ children }: { children: React.ReactNode }) => <div data-testid="archive-marker">{children}</div>,
  TileLayer: () => <div data-testid="tile-layer" />,
  Polyline: () => <div data-testid="polyline" />,
  Popup: ({ children }: { children: React.ReactNode }) => <div>{children}</div>,
}))

vi.mock('sonner', () => ({
  toast: { error: vi.fn() },
}))

vi.mock('../../../../infrastructure/api/routesApi', () => ({
  routesApi: {
    getArchiveRoutes: vi.fn(),
    getRouteDetails: vi.fn(),
  },
}))

vi.mock('../../../../infrastructure/api/users/usersApi', () => ({
  getUsers: vi.fn(),
}))

import { routesApi } from '../../../../infrastructure/api/routesApi'
import { getUsers } from '../../../../infrastructure/api/users/usersApi'

const archivedRoute: RouteResponse = {
  id: 'route-1',
  postmanId: 'postman-1',
  postmanName: 'Postar Test',
  date: '2026-05-30',
  plannedStartTime: '08:00:00',
  plannedEndTime: '10:00:00',
  totalDistanceKm: 12.5,
  totalDurationMinutes: 120,
  status: 'Zavrsena',
  exceedsStandardTime: false,
  lastReorderedAt: null,
  lastReorderedBy: null,
  assignedAt: '2026-05-30T07:30:00Z',
  assignedBy: 'dispatcher',
  startedAt: '2026-05-30T08:05:00Z',
  completedAt: '2026-05-30T10:00:00Z',
  routeItems: [
    {
      id: 'item-1',
      mailboxId: 'mailbox-1',
      address: 'Titova 1',
      latitude: 43.8563,
      longitude: 18.4131,
      order: 1,
      estimatedArrivalTime: '08:15:00',
      priority: 'Visok',
      status: 'Nedostupan',
      isManuallyReordered: false,
      mailboxStatus: 'Nedostupan',
      processedAt: '2026-05-30T08:20:00Z',
      processedBy: 'postman-1',
      processedStatus: 'Nedostupan',
      unavailableReason: 'Zakljucan pristup',
    },
  ],
}

describe('ArchiveRoute pages - PBI-034/PBI-035', () => {
  beforeEach(() => {
    vi.clearAllMocks()
    vi.mocked(getUsers).mockResolvedValue({
      data: [
        {
          id: 'postman-1',
          username: 'postar.test',
          email: 'postar@test.ba',
          role: 'PostalWorker',
          mustChangePassword: false,
          isLockedOut: false,
        },
      ],
      status: 200,
    })
    vi.mocked(routesApi.getArchiveRoutes).mockResolvedValue({
      items: [archivedRoute],
      totalCount: 1,
      page: 1,
      pageSize: 20,
      totalPages: 1,
    })
    vi.mocked(routesApi.getRouteDetails).mockResolvedValue(archivedRoute)
  })

  it('ucitava arhivu i filtrira po periodu i postaru', async () => {
    const user = userEvent.setup()
    render(
      <BrowserRouter>
        <ArchiveRouteListPage />
      </BrowserRouter>
    )

    await screen.findByText('Postar Test')
    await screen.findByRole('option', { name: 'postar.test' })
    fireEvent.change(screen.getByLabelText(/Od datuma/i), { target: { value: '2026-05-01' } })
    fireEvent.change(screen.getByLabelText(/Do datuma/i), { target: { value: '2026-05-30' } })
    await user.selectOptions(screen.getByLabelText(/Postar/i), 'postman-1')

    await waitFor(() => {
      expect(vi.mocked(routesApi.getArchiveRoutes).mock.calls.at(-1)).toEqual([
        1,
        20,
        '2026-05-01',
        '2026-05-30',
        'postman-1',
      ])
    })
  })

  it('prikazuje read-only detalje, timestamp, finalni status, razlog i mapu', async () => {
    render(
      <MemoryRouter initialEntries={['/admin/routes/archive/route-1']}>
        <Routes>
          <Route path="/admin/routes/archive/:id" element={<ArchiveRouteDetailsPage />} />
        </Routes>
      </MemoryRouter>
    )

    expect(await screen.findByText(/Read-only arhivski pregled/i)).toBeInTheDocument()
    expect(screen.getByText('Titova 1')).toBeInTheDocument()
    expect(screen.getAllByText('Nedostupan').length).toBeGreaterThan(0)
    expect(screen.getByText('Zakljucan pristup')).toBeInTheDocument()
    expect(screen.getByText('2026-05-30T08:20:00Z')).toBeInTheDocument()
    expect(screen.getByTestId('archive-map')).toBeInTheDocument()
    expect(screen.queryByRole('button', { name: /sacuvaj|uredi|izmijeni/i })).not.toBeInTheDocument()
  })

  it('exportuje arhivirane detalje kao CSV koji Excel moze otvoriti', async () => {
    const user = userEvent.setup()
    const createObjectURL = vi.spyOn(URL, 'createObjectURL').mockReturnValue('blob:archive')
    const revokeObjectURL = vi.spyOn(URL, 'revokeObjectURL').mockImplementation(() => undefined)
    const click = vi.spyOn(HTMLAnchorElement.prototype, 'click').mockImplementation(() => undefined)

    render(
      <MemoryRouter initialEntries={['/admin/routes/archive/route-1']}>
        <Routes>
          <Route path="/admin/routes/archive/:id" element={<ArchiveRouteDetailsPage />} />
        </Routes>
      </MemoryRouter>
    )

    await screen.findByText('Titova 1')
    await user.click(screen.getByRole('button', { name: /Export CSV za Excel/i }))

    expect(createObjectURL).toHaveBeenCalled()
    expect(click).toHaveBeenCalled()

    createObjectURL.mockRestore()
    revokeObjectURL.mockRestore()
    click.mockRestore()
  })
})
