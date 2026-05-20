import { render, screen, waitFor, within } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { BrowserRouter } from 'react-router-dom'
import { beforeEach, describe, expect, it, vi } from 'vitest'
import GenerateRoutePage from '../GenerateRoutePage'

vi.mock('leaflet', () => ({
  default: {
    Icon: {
      Default: {
        prototype: {},
        mergeOptions: vi.fn(),
      },
    },
  },
}))

vi.mock('react-leaflet', () => ({
  MapContainer: ({ children }: { children: React.ReactNode }) => <div data-testid="route-map">{children}</div>,
  TileLayer: () => <div data-testid="tile-layer" />,
  Marker: ({ children }: { children: React.ReactNode }) => <div data-testid="route-marker">{children}</div>,
  Popup: ({ children }: { children: React.ReactNode }) => <div>{children}</div>,
  Circle: () => <div data-testid="selected-circle" />,
}))

vi.mock('../../../components/common/LeafletRoutingMachine', () => ({
  LeafletRoutingMachine: () => <div data-testid="routing-line" />,
}))

vi.mock('../../../components/Layout/Layout', () => ({
  Layout: ({ children }: { children: React.ReactNode }) => <div>{children}</div>,
}))

vi.mock('../../../../infrastructure/api/users/usersApi', () => ({
  getUsers: vi.fn(),
}))

vi.mock('../../../../infrastructure/api/routesApi', () => ({
  routesApi: {
    generateRoute: vi.fn(),
    getAvailablePostmen: vi.fn(),
    assignRoute: vi.fn(),
    reorderRoute: vi.fn(),
  },
}))

vi.mock('sonner', () => ({
  toast: { success: vi.fn(), error: vi.fn(), warning: vi.fn() },
}))

import { routesApi } from '../../../../infrastructure/api/routesApi'
import { getUsers } from '../../../../infrastructure/api/users/usersApi'
import { toast } from 'sonner'

const generatedRoute = {
  id: 'route-1',
  postmanId: 'seed-postman',
  postmanName: null,
  date: '2026-05-20',
  plannedStartTime: '08:00:00',
  plannedEndTime: '10:00:00',
  totalDistanceKm: 12.5,
  totalDurationMinutes: 120,
  status: 'Planirana',
  exceedsStandardTime: false,
  lastReorderedAt: null,
  lastReorderedBy: null,
  assignedAt: null,
  assignedBy: null,
  routeItems: [
    {
      id: 'item-1',
      mailboxId: 'mailbox-1',
      address: 'Titova 1, Sarajevo',
      latitude: 43.8563,
      longitude: 18.4131,
      order: 1,
      estimatedArrivalTime: '08:15:00',
      priority: 'Visok',
      status: 'Planirano',
      isManuallyReordered: false,
    },
  ],
}

const assignedRoute = {
  ...generatedRoute,
  postmanId: 'postman-1',
  postmanName: 'Amar Hodzic',
  status: 'Dodijeljena',
  assignedAt: '2026-05-20T13:30:00Z',
  assignedBy: 'dispatcher',
}

const availability = [
  {
    id: 'postman-1',
    fullName: 'Amar Hodzic',
    username: 'amar.hodzic',
    email: 'amar@postroute.ba',
    isAvailable: true,
    isCurrentAssignee: false,
    unavailableReason: null,
  },
  {
    id: 'postman-2',
    fullName: 'Tarik Music',
    username: 'tarik.music',
    email: 'tarik@postroute.ba',
    isAvailable: false,
    isCurrentAssignee: false,
    unavailableReason: 'Postar vec ima dodijeljenu rutu za ovaj datum.',
  },
]

function renderPage() {
  return render(
    <BrowserRouter>
      <GenerateRoutePage />
    </BrowserRouter>
  )
}

async function generateRoute(user = userEvent.setup()) {
  renderPage()
  await screen.findByRole('option', { name: /planner/i })
  await user.selectOptions(screen.getByLabelText(/poštar/i), 'seed-postman')
  await user.click(screen.getByRole('button', { name: /generiši rutu/i }))
  await screen.findByRole('button', { name: /dodijeli poštaru/i })
}

describe('GenerateRoutePage — PBI-023 dodjela rute', () => {
  beforeEach(() => {
    vi.clearAllMocks()
    vi.mocked(getUsers).mockResolvedValue({
      data: [
        {
          id: 'seed-postman',
          username: 'planner',
          email: 'planner@postroute.ba',
          role: 'PostalWorker',
          mustChangePassword: false,
          isLockedOut: false,
        },
      ],
      status: 200,
    })
    vi.mocked(routesApi.generateRoute).mockResolvedValue(generatedRoute)
    vi.mocked(routesApi.getAvailablePostmen).mockResolvedValue(availability)
    vi.mocked(routesApi.assignRoute).mockResolvedValue(assignedRoute)
  })

  it('prikazuje dugme Dodijeli poštaru nakon generisanja prijedloga rute', async () => {
    await generateRoute()

    expect(screen.getAllByText(/prijedlog/i).length).toBeGreaterThan(0)
    expect(screen.getByRole('button', { name: /dodijeli poštaru/i })).toBeInTheDocument()
    expect(routesApi.getAvailablePostmen).toHaveBeenCalledWith('route-1')
  })

  it('prikazuje dropdown sa zauzetim poštarom kao onemogućenim izborom', async () => {
    const user = userEvent.setup()
    await generateRoute(user)

    await user.click(screen.getByRole('button', { name: /dodijeli poštaru/i }))

    const assigneeSelect = await screen.findByLabelText(/poštar/i, { selector: '#assigneeId' })
    expect(within(assigneeSelect).getByRole('option', { name: /Amar Hodzic/i })).toBeEnabled()
    expect(within(assigneeSelect).getByRole('option', { name: /Tarik Music/i })).toBeDisabled()
    expect(screen.getByText(/već imaju dodijeljenu rutu/i)).toBeInTheDocument()
  })

  it('dodjeljuje rutu odabranom poštaru i prikazuje toast uspjeha', async () => {
    const user = userEvent.setup()
    await generateRoute(user)

    await user.click(screen.getByRole('button', { name: /dodijeli poštaru/i }))
    const assigneeSelect = await screen.findByLabelText(/poštar/i, { selector: '#assigneeId' })
    await user.selectOptions(assigneeSelect, 'postman-1')
    await user.click(screen.getByRole('button', { name: /potvrdi dodjelu/i }))

    await waitFor(() => {
      expect(routesApi.assignRoute).toHaveBeenCalledWith('route-1', 'postman-1')
    })
    expect(toast.success).toHaveBeenCalledWith(expect.stringContaining('Amar Hodzic'))
    expect((await screen.findAllByText(/dodijeljena/i)).length).toBeGreaterThan(0)
  })

  it('prikazuje poruku kada nema slobodnih poštara za datum rute', async () => {
    const user = userEvent.setup()
    vi.mocked(routesApi.getAvailablePostmen).mockResolvedValue([
      { ...availability[1] },
    ])

    await generateRoute(user)
    await user.click(screen.getByRole('button', { name: /dodijeli poštaru/i }))

    expect(await screen.findByText(/nema dostupnih poštara za odabrani datum/i)).toBeInTheDocument()
  })
})
