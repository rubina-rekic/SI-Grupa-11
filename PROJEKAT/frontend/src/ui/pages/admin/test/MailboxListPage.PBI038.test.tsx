import { render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { BrowserRouter } from 'react-router-dom'
import { beforeEach, describe, expect, it, vi } from 'vitest'
import MailboxListPage from '../MailboxListPage'

vi.mock('../../../components/Layout/Layout', () => ({
  Layout: ({ children }: { children: React.ReactNode }) => <div>{children}</div>,
}))

vi.mock('react-leaflet', () => ({
  MapContainer: ({ children }: { children: React.ReactNode }) => <div data-testid="map">{children}</div>,
  Marker: () => <div data-testid="marker" />,
  TileLayer: () => <div data-testid="tile-layer" />,
}))

vi.mock('sonner', () => ({
  toast: { error: vi.fn() },
}))

vi.mock('../../../../infrastructure/api/mailboxes/mailboxesApi', () => ({
  getAllMailboxes: vi.fn(),
  MailboxType: { WallSmall: 1, StandaloneLarge: 2, IndoorResidential: 3, SpecialPriority: 4 },
  MailboxPriority: { Visok: 1, Srednji: 2, Nizak: 3 },
  MailboxStatus: { Prazan: 0, Pun: 1, Obraen: 2, Napunjen: 3, Ispraznjen: 4, Nedostupan: 5 },
  mailboxTypeLabels: {
    1: 'Zidni (mali)',
    2: 'Samostojeci (veliki)',
    3: 'Unutrasnji (stambene zgrade)',
    4: 'Specijalni (prioritetni)',
  },
  mailboxPriorityLabels: { 1: 'Visok', 2: 'Srednji', 3: 'Nizak' },
  mailboxStatusLabels: {
    0: 'Prazan',
    1: 'Pun',
    2: 'Obradjen',
    3: 'Napunjen',
    4: 'Ispraznjen',
    5: 'Nedostupan',
  },
}))

import { getAllMailboxes } from '../../../../infrastructure/api/mailboxes/mailboxesApi'
import type { MailboxResponse } from '../../../../infrastructure/api/mailboxes/mailboxesApi'

const mailbox: MailboxResponse = {
  id: 'mailbox-1',
  serialNumber: 'SN001',
  address: 'Titova 1',
  latitude: 43.8563,
  longitude: 18.4131,
  type: 1 as const,
  priority: 1 as const,
  status: 1 as const,
  capacity: 100,
  installationYear: 2024,
  createdAt: '2026-05-30T08:00:00Z',
  updatedAt: '2026-05-30T08:00:00Z',
  notes: '',
  isAlwaysAvailable: true,
  slot1Start: null,
  slot1End: null,
  slot2Start: null,
  slot2End: null,
  workingDays: 31 as const,
}

const pagedResult = {
  items: [mailbox],
  totalCount: 1,
  page: 1,
  pageSize: 25,
  totalPages: 1,
}

const emptyResult = {
  items: [],
  totalCount: 0,
  page: 1,
  pageSize: 25,
  totalPages: 0,
}

function renderPage() {
  return render(
    <BrowserRouter>
      <MailboxListPage />
    </BrowserRouter>
  )
}

describe('MailboxListPage - PBI-038 brza pretraga sandučića', () => {
  beforeEach(() => {
    vi.clearAllMocks()
    vi.mocked(getAllMailboxes).mockResolvedValue(pagedResult)
  })

  it('ne aktivira pretragu kada korisnik unese manje od 3 karaktera', async () => {
    const user = userEvent.setup()
    renderPage()
    await screen.findByText('Titova 1')

    const callsBefore = vi.mocked(getAllMailboxes).mock.calls.length
    await user.type(screen.getByLabelText(/Pretraga/i), 'SN')

    // cekamo debounce; ako se poziv dogodi, search mora biti undefined
    await new Promise(resolve => setTimeout(resolve, 400))
    const newCalls = vi.mocked(getAllMailboxes).mock.calls.slice(callsBefore)
    const anyWithSearch = newCalls.some(c => c[0]?.search !== undefined)
    expect(anyWithSearch).toBe(false)
  })

  it('aktivira pretragu kada korisnik unese 3 ili vise karaktera', async () => {
    const user = userEvent.setup()
    renderPage()
    await screen.findByText('Titova 1')

    await user.type(screen.getByLabelText(/Pretraga/i), 'SN0')

    await waitFor(() => {
      expect(vi.mocked(getAllMailboxes).mock.calls.at(-1)?.[0]).toMatchObject({
        search: 'SN0',
      })
    }, { timeout: 1500 })
  })

  it('prikazuje poruku kada pretraga ne vrati rezultate', async () => {
    const user = userEvent.setup()
    vi.mocked(getAllMailboxes)
      .mockResolvedValueOnce(pagedResult)
      .mockResolvedValue(emptyResult)

    renderPage()
    await screen.findByText('Titova 1')
    await user.type(screen.getByLabelText(/Pretraga/i), 'xyz')

    expect(
      await screen.findByText('Nema pronađenih sandučića za uneseni pojam.', {}, { timeout: 1500 })
    ).toBeInTheDocument()
  })

  it('vraca punu listu kada se polje za pretragu isprazni', async () => {
    const user = userEvent.setup()
    renderPage()
    await screen.findByText('Titova 1')

    await user.type(screen.getByLabelText(/Pretraga/i), 'SN0')
    await waitFor(() => {
      expect(vi.mocked(getAllMailboxes).mock.calls.at(-1)?.[0]?.search).toBe('SN0')
    }, { timeout: 1500 })

    await user.clear(screen.getByLabelText(/Pretraga/i))
    await waitFor(() => {
      const lastQuery = vi.mocked(getAllMailboxes).mock.calls.at(-1)?.[0]
      expect(lastQuery?.search).toBeUndefined()
    }, { timeout: 1500 })
  })

  it('podrzava parcijalno pretrazivanje po adresi', async () => {
    const user = userEvent.setup()
    renderPage()
    await screen.findByText('Titova 1')

    await user.type(screen.getByLabelText(/Pretraga/i), 'Tit')

    await waitFor(() => {
      expect(vi.mocked(getAllMailboxes).mock.calls.at(-1)?.[0]).toMatchObject({
        search: 'Tit',
      })
    }, { timeout: 1500 })
  })
})
