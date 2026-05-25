import { render, screen, within } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { BrowserRouter } from 'react-router-dom'
import { beforeEach, describe, expect, it, vi } from 'vitest'
import DispatcherRouteDashboardPage from '../DispatcherRouteDashboardPage'
import type { RouteResponse } from '../../../../infrastructure/api/routesApi'

vi.mock('../../../components/Layout/Layout', () => ({
  Layout: ({ children }: { children: React.ReactNode }) => <div>{children}</div>,
}))

vi.mock('../../../../infrastructure/api/routesApi', () => ({
  routesApi: {
    getRoutesForDate: vi.fn(),
  },
}))

vi.mock('../../../../infrastructure/api/users/usersApi', () => ({
  getUsers: vi.fn(),
}))

vi.mock('sonner', () => ({
  toast: { error: vi.fn() },
}))

import { routesApi } from '../../../../infrastructure/api/routesApi'
import { getUsers } from '../../../../infrastructure/api/users/usersApi'

const postmen = [
  {
    id: 'postman-1',
    username: 'postar.user',
    email: 'postar@postroute.ba',
    role: 'PostalWorker',
    mustChangePassword: false,
    isLockedOut: false,
  },
  {
    id: 'postman-2',
    username: 'drugi.postar',
    email: 'drugi@postroute.ba',
    role: 'PostalWorker',
    mustChangePassword: false,
    isLockedOut: false,
  },
]

function makeRoute(overrides: Partial<RouteResponse> = {}): RouteResponse {
  return {
    id: '11111111-1111-1111-1111-111111111111',
    postmanId: 'postman-1',
    postmanName: 'Postar User',
    date: '2026-05-24',
    plannedStartTime: '08:00:00',
    plannedEndTime: '10:00:00',
    totalDistanceKm: 12.5,
    totalDurationMinutes: 120,
    status: 'UProgresu',
    exceedsStandardTime: false,
    lastReorderedAt: null,
    lastReorderedBy: null,
    assignedAt: '2026-05-24T07:30:00Z',
    assignedBy: 'dispatcher',
    startedAt: '2026-05-24T08:05:00Z',
    completedAt: null,
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
        status: 'Planirano',
        isManuallyReordered: false,
        mailboxStatus: 'Ispraznjen',
        processedAt: '2026-05-24T08:20:00Z',
        processedBy: 'postman-1',
        processedStatus: 'Ispraznjen',
      },
      {
        id: 'item-2',
        mailboxId: 'mailbox-2',
        address: 'Zmaja od Bosne 2',
        latitude: 43.858,
        longitude: 18.41,
        order: 2,
        estimatedArrivalTime: '08:35:00',
        priority: 'Srednji',
        status: 'Nedostupan',
        isManuallyReordered: false,
        mailboxStatus: 'Prazan',
        processedAt: null,
        processedBy: null,
        processedStatus: null,
      },
      {
        id: 'item-3',
        mailboxId: 'mailbox-3',
        address: 'Ferhadija 3',
        latitude: 43.859,
        longitude: 18.42,
        order: 3,
        estimatedArrivalTime: '08:55:00',
        priority: 'Nizak',
        status: 'Planirano',
        isManuallyReordered: false,
        mailboxStatus: 'Prazan',
        processedAt: null,
        processedBy: null,
        processedStatus: null,
      },
    ],
    ...overrides,
  }
}

function renderDashboard() {
  return render(
    <BrowserRouter>
      <DispatcherRouteDashboardPage />
    </BrowserRouter>
  )
}

async function generateReport() {
  const user = userEvent.setup()
  await screen.findByRole('option', { name: 'postar.user' })
  await user.selectOptions(screen.getByRole('combobox'), 'postman-1')
  await user.click(screen.getByRole('button', { name: /gener/i }))
  await screen.findByRole('heading', { name: /Dnevna ruta/i })
  return user
}

describe('DispatcherRouteDashboardPage - PBI-030 dnevni izvjestaj', () => {
  beforeEach(() => {
    vi.clearAllMocks()
    vi.mocked(getUsers).mockResolvedValue({ data: postmen, status: 200 })
    vi.mocked(routesApi.getRoutesForDate).mockResolvedValue([makeRoute()])
  })

  it('generise dnevni izvjestaj sa zaglavljem, sumarnim blokom i detaljnom tabelom', async () => {
    renderDashboard()

    await generateReport()

    expect(screen.getByRole('heading', { name: /Dnevni/i })).toBeInTheDocument()
    expect(screen.getByRole('heading', { name: /Dnevna ruta 11111111/i })).toBeInTheDocument()
    expect(screen.getAllByText(/Postar User/i).length).toBeGreaterThan(0)
    expect(screen.getByText('33%')).toBeInTheDocument()
    const table = screen.getByRole('table')
    expect(within(table).getByText('Titova 1')).toBeInTheDocument()
    expect(within(table).getByText('Zmaja od Bosne 2')).toBeInTheDocument()
    expect(within(table).getByText('Ferhadija 3')).toBeInTheDocument()
    expect(within(table).getByText(/Ispraznjen/i)).toBeInTheDocument()
    expect(within(table).getByText(/Nedostupan/i)).toBeInTheDocument()
    expect(within(table).getByText(/Nije posje/i)).toBeInTheDocument()
    const rows = within(table).getAllByRole('row')
    expect(rows[1]).toHaveClass('rdb-report-row--processed')
    expect(rows[2]).toHaveClass('rdb-report-row--unavailable')
    expect(rows[3]).toHaveClass('rdb-report-row--unvisited')
  })

  it('prikazuje upozorenje kada je realizacija ispod 80 posto', async () => {
    renderDashboard()

    await generateReport()

    expect(screen.getByText(/Realizacija rute ispod standardnog praga/i)).toBeInTheDocument()
  })

  it('prikazuje poruku kada za odabrani datum i postara nema rute', async () => {
    const user = userEvent.setup()
    vi.mocked(routesApi.getRoutesForDate).mockResolvedValue([
      makeRoute({ id: '22222222-2222-2222-2222-222222222222', postmanId: 'postman-2', postmanName: 'Drugi Postar' }),
    ])
    renderDashboard()

    await screen.findByRole('option', { name: 'postar.user' })
    await user.selectOptions(screen.getByRole('combobox'), 'postman-1')
    await user.click(screen.getByRole('button', { name: /gener/i }))

    expect(screen.getByText('Nema podataka za odabrane parametre.')).toBeInTheDocument()
  })

  it('u izvjestaju i kartici prikazuje Zavrsena kada su svi sanducici obradeni', async () => {
    const completedRoute = makeRoute({
      status: 'Dodijeljena',
      completedAt: null,
      routeItems: makeRoute().routeItems.map((item, index) => ({
        ...item,
        status: 'Planirano',
        mailboxStatus: index === 0 ? 'Ispraznjen' : 'Napunjen',
        processedAt: `2026-05-24T09:0${index}:00Z`,
        processedBy: 'postman-1',
        processedStatus: index === 0 ? 'Ispraznjen' : 'Napunjen',
      })),
    })
    vi.mocked(routesApi.getRoutesForDate).mockResolvedValue([completedRoute])
    renderDashboard()

    await generateReport()

    expect(screen.getAllByText(/Zavr/i).length).toBeGreaterThan(0)
    expect(screen.getByText('100%')).toBeInTheDocument()
    expect(screen.queryByText(/Realizacija rute ispod standardnog praga/i)).not.toBeInTheDocument()
  })

  it('otvara print-friendly PDF prikaz sa podacima izvjestaja', async () => {
    const user = userEvent.setup()
    const write = vi.fn()
    const close = vi.fn()
    const focus = vi.fn()
    const print = vi.fn()
    const setTimeout = vi.fn((callback: () => void) => callback())
    const openSpy = vi.spyOn(window, 'open').mockReturnValue({
      document: { write, close },
      focus,
      print,
      setTimeout,
    } as unknown as Window)
    renderDashboard()

    await generateReport()
    await user.click(screen.getByRole('button', { name: /Preuzmi PDF/i }))

    expect(openSpy).toHaveBeenCalled()
    expect(write).toHaveBeenCalledWith(expect.stringContaining('Dnevna ruta 11111111'))
    expect(write).toHaveBeenCalledWith(expect.stringContaining('Titova 1'))
    expect(write).toHaveBeenCalledWith(expect.stringContaining('Realizacija'))
    expect(print).toHaveBeenCalled()
  })
})
