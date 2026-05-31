import { fireEvent, render, screen, waitFor, within } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { beforeEach, describe, expect, it, vi } from 'vitest'
import PostmanPerformanceReportPage from '../PostmanPerformanceReportPage'
import type { PostmanPerformanceReportResponse } from '../../../../infrastructure/api/routesApi'

vi.mock('../../../components/Layout/Layout', () => ({
  Layout: ({ children }: { children: React.ReactNode }) => <div>{children}</div>,
}))

vi.mock('sonner', () => ({
  toast: { error: vi.fn() },
}))

vi.mock('../../../../infrastructure/api/routesApi', () => ({
  routesApi: {
    getPostmanPerformanceReport: vi.fn(),
  },
}))

import { routesApi } from '../../../../infrastructure/api/routesApi'

const report: PostmanPerformanceReportResponse = {
  fromDate: '2026-05-01',
  toDate: '2026-05-31',
  totalPostmen: 2,
  totalAssignedMailboxes: 5,
  totalEmptiedLocations: 3,
  totalUnrealizedLocations: 2,
  teamAverageSuccessPercentage: 58.34,
  rows: [
    {
      postmanId: 'postman-low',
      postmanName: 'Aldin Test',
      assignedMailboxes: 2,
      emptiedLocations: 1,
      unrealizedLocations: 1,
      successPercentage: 50,
      completedRoutesCount: 1,
      routes: [
        {
          routeId: 'route-low',
          date: '2026-05-12',
          plannedStartTime: '09:00:00',
          completedAt: '2026-05-12T11:00:00Z',
          assignedMailboxes: 2,
          emptiedLocations: 1,
          unrealizedLocations: 1,
          successPercentage: 50,
        },
      ],
    },
    {
      postmanId: 'postman-high',
      postmanName: 'Ibrahim Test',
      assignedMailboxes: 3,
      emptiedLocations: 2,
      unrealizedLocations: 1,
      successPercentage: 66.67,
      completedRoutesCount: 1,
      routes: [
        {
          routeId: 'route-high',
          date: '2026-05-10',
          plannedStartTime: '08:00:00',
          completedAt: '2026-05-10T10:00:00Z',
          assignedMailboxes: 3,
          emptiedLocations: 2,
          unrealizedLocations: 1,
          successPercentage: 66.67,
        },
      ],
    },
  ],
}

describe('PostmanPerformanceReportPage - PBI-050 / US-36', () => {
  beforeEach(() => {
    vi.clearAllMocks()
    vi.mocked(routesApi.getPostmanPerformanceReport).mockResolvedValue(report)
  })

  it('prikazuje KPI tabelu, procenat uspjesnosti i stubni grafikon', async () => {
    render(<PostmanPerformanceReportPage />)

    expect(await screen.findByText('KPI tabela')).toBeInTheDocument()
    expect(screen.getByText('Poređenje učinka')).toBeInTheDocument()
    expect(screen.getByRole('img', { name: /Stubni grafikon uspješnosti/i })).toBeInTheDocument()
    expect(screen.getAllByText('Ibrahim Test').length).toBeGreaterThan(0)
    expect(screen.getAllByText('Aldin Test').length).toBeGreaterThan(0)
    expect(screen.getAllByText('66.67%').length).toBeGreaterThan(0)
    expect(screen.getAllByText('50%').length).toBeGreaterThan(0)
    expect(screen.getByText('Prosjek tima')).toBeInTheDocument()
    expect(screen.getByText('58.34%')).toBeInTheDocument()
  })

  it('filtrira izvjestaj prema odabranom periodu', async () => {
    const user = userEvent.setup()
    render(<PostmanPerformanceReportPage />)

    await screen.findByText('KPI tabela')
    fireEvent.change(screen.getByLabelText(/Od datuma/i), { target: { value: '2026-05-01' } })
    fireEvent.change(screen.getByLabelText(/Do datuma/i), { target: { value: '2026-05-31' } })
    await user.click(screen.getByRole('button', { name: /Prikaži izvještaj/i }))

    await waitFor(() => {
      expect(vi.mocked(routesApi.getPostmanPerformanceReport).mock.calls.at(-1)).toEqual([
        '2026-05-01',
        '2026-05-31',
      ])
    })
  })

  it('sortira tabelu po uspjesnosti i otvara detalje ruta klikom na ime postara', async () => {
    const user = userEvent.setup()
    render(<PostmanPerformanceReportPage />)

    await screen.findByText('KPI tabela')
    let nameButtons = screen.getAllByRole('button', { name: /Test/i })
    expect(nameButtons[0]).toHaveTextContent('Ibrahim Test')

    await user.click(screen.getByRole('button', { name: /Uspješnost/i }))
    nameButtons = screen.getAllByRole('button', { name: /Test/i })
    expect(nameButtons[0]).toHaveTextContent('Aldin Test')

    await user.click(screen.getByRole('button', { name: 'Aldin Test' }))
    const details = screen.getByLabelText(/Detalji ruta za Aldin Test/i)
    expect(within(details).getByText(/Rute u obračunu: Aldin Test/i)).toBeInTheDocument()
    expect(within(details).getByText('2')).toBeInTheDocument()
    expect(within(details).getAllByText('1').length).toBeGreaterThanOrEqual(2)
  })

  it('exportuje sumarni izvjestaj u CSV', async () => {
    const user = userEvent.setup()
    const createObjectURL = vi.spyOn(URL, 'createObjectURL').mockReturnValue('blob:report')
    const revokeObjectURL = vi.spyOn(URL, 'revokeObjectURL').mockImplementation(() => undefined)
    const click = vi.spyOn(HTMLAnchorElement.prototype, 'click').mockImplementation(() => undefined)

    render(<PostmanPerformanceReportPage />)

    await screen.findByText('KPI tabela')
    await user.click(screen.getByRole('button', { name: /Export CSV/i }))

    expect(createObjectURL).toHaveBeenCalled()
    expect(click).toHaveBeenCalled()

    createObjectURL.mockRestore()
    revokeObjectURL.mockRestore()
    click.mockRestore()
  })
})
