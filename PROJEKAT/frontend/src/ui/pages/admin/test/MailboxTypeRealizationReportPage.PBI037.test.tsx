import { fireEvent, render, screen, waitFor, within } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { beforeEach, describe, expect, it, vi } from 'vitest'
import MailboxTypeRealizationReportPage from '../MailboxTypeRealizationReportPage'
import type { MailboxTypeRealizationReportResponse } from '../../../../infrastructure/api/routesApi'

vi.mock('../../../components/Layout/Layout', () => ({
  Layout: ({ children }: { children: React.ReactNode }) => <div>{children}</div>,
}))

vi.mock('sonner', () => ({
  toast: { error: vi.fn() },
}))

vi.mock('../../../../infrastructure/api/routesApi', () => ({
  routesApi: {
    getMailboxTypeRealizationReport: vi.fn(),
  },
}))

import { routesApi } from '../../../../infrastructure/api/routesApi'

const report: MailboxTypeRealizationReportResponse = {
  fromDate: '2026-05-01',
  toDate: '2026-05-31',
  totalTypes: 2,
  totalPlannedEmpties: 4,
  totalSuccessfulEmpties: 2,
  totalProblemReports: 2,
  averageFailureRate: 50,
  rows: [
    {
      typeId: 1,
      typeName: 'WallSmall',
      plannedEmpties: 2,
      successfulEmpties: 1,
      problemReports: 1,
      failureRate: 50,
      details: [
        {
          mailboxId: 'mailbox-1',
          address: 'Adresa 1',
          routeDate: '2026-05-10',
          status: 'Nedostupan',
          notes: 'Nema pristupa',
        },
      ],
    },
    {
      typeId: 2,
      typeName: 'StandaloneLarge',
      plannedEmpties: 2,
      successfulEmpties: 1,
      problemReports: 1,
      failureRate: 50,
      details: [
        {
          mailboxId: 'mailbox-2',
          address: 'Adresa 2',
          routeDate: '2026-05-15',
          status: 'Nedostupan',
          notes: 'Zatvoreno',
        },
      ],
    },
  ],
}

describe('MailboxTypeRealizationReportPage - PBI-037', () => {
  beforeEach(() => {
    vi.clearAllMocks()
    vi.mocked(routesApi.getMailboxTypeRealizationReport).mockResolvedValue(report)
  })

  it('prikazuje izvještaj sa tabelom i detaljima po tipu sandučića', async () => {
    render(<MailboxTypeRealizationReportPage />)

    expect(await screen.findByText('Analiza realizacije po tipu sandučića')).toBeInTheDocument()
    expect(screen.getByText('Distribucija planiranih isprazni')).toBeInTheDocument()
    expect(screen.getByText('Tip sandučića')).toBeInTheDocument()
    expect(screen.getAllByText('Zidni (mali)').length).toBeGreaterThanOrEqual(1)
    expect(screen.getAllByText('Samostojeći (veliki)').length).toBeGreaterThanOrEqual(1)

    const plannedMetric = screen.getByText('Planirani isprazni').closest('div')
    expect(within(plannedMetric as HTMLElement).getByText('4')).toBeInTheDocument()

    const successfulMetric = screen.getByText('Uspješni isprazni').closest('div')
    expect(within(successfulMetric as HTMLElement).getByText('2')).toBeInTheDocument()

    const failureMetric = screen.getByText('Prosjek neuspjeha').closest('div')
    expect(within(failureMetric as HTMLElement).getByText('50.00%')).toBeInTheDocument()
  })

  it('filtrira period i učitava izvještaj ponovo', async () => {
    const user = userEvent.setup()
    render(<MailboxTypeRealizationReportPage />)

    await screen.findByText('Tip sandučića')
    fireEvent.change(screen.getByLabelText(/Od datuma/i), { target: { value: '2026-05-01' } })
    fireEvent.change(screen.getByLabelText(/Do datuma/i), { target: { value: '2026-05-31' } })
    await user.click(screen.getByRole('button', { name: /Prikaži izvještaj/i }))

    await waitFor(() => {
      expect(vi.mocked(routesApi.getMailboxTypeRealizationReport).mock.calls.at(-1)).toEqual([
        '2026-05-01',
        '2026-05-31',
      ])
    })
  })

  it('otvara detalje tipa sandučića i prikazuje bilješke', async () => {
    const user = userEvent.setup()
    render(<MailboxTypeRealizationReportPage />)

    await screen.findByText('Tip sandučića')
    await user.click(screen.getByRole('button', { name: 'Zidni (mali)' }))

    const detail = screen.getByRole('heading', { name: /Detalji za Zidni/i })
    const detailSection = detail.closest('section')
    expect(detailSection).not.toBeNull()
    expect(within(detailSection as HTMLElement).getByText('Adresa 1')).toBeInTheDocument()
    expect(within(detailSection as HTMLElement).getByText(/10\.\s?05\.\s?2026/i)).toBeInTheDocument()
    expect(within(detailSection as HTMLElement).getByText('Nedostupan')).toBeInTheDocument()
    expect(within(detailSection as HTMLElement).getByText('Nema pristupa')).toBeInTheDocument()
  })

  it('exportuje izvještaj u CSV', async () => {
    const user = userEvent.setup()
    const createObjectURL = vi.spyOn(URL, 'createObjectURL').mockReturnValue('blob:report')
    const revokeObjectURL = vi.spyOn(URL, 'revokeObjectURL').mockImplementation(() => undefined)
    const click = vi.spyOn(HTMLAnchorElement.prototype, 'click').mockImplementation(() => undefined)

    render(<MailboxTypeRealizationReportPage />)

    await screen.findByText('Tip sandučića')
    await user.click(screen.getByRole('button', { name: /Export CSV/i }))

    expect(createObjectURL).toHaveBeenCalled()
    expect(click).toHaveBeenCalled()

    createObjectURL.mockRestore()
    revokeObjectURL.mockRestore()
    click.mockRestore()
  })
})
