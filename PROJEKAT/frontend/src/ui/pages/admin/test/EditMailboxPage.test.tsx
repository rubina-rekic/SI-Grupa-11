import { render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { vi, describe, it, expect, beforeEach } from 'vitest'
import { BrowserRouter } from 'react-router-dom'
import EditMailboxPage from '../EditMailboxPage'

const { mockMailbox } = vi.hoisted(() => ({
  mockMailbox: {
    id: 'test-id-123',
    serialNumber: 'SN001',
    address: 'Titova 1, Sarajevo',
    latitude: 43.8563,
    longitude: 18.4131,
    type: 1,
    priority: 2,
    status: 1,
    capacity: 100,
    installationYear: 2020,
    notes: 'Test napomena',
    isAlwaysAvailable: false,
    createdAt: '2024-01-01T00:00:00Z',
    updatedAt: '2024-01-01T00:00:00Z',
    slot1Start: '08:00:00',
    slot1End: '16:00:00',
    slot2Start: null,
    slot2End: null,
  }
}))

const mockNavigate = vi.fn()
vi.mock('react-router-dom', async () => {
  const actual = await vi.importActual('react-router-dom')
  return { ...actual, useNavigate: () => mockNavigate, useParams: () => ({ id: 'test-id-123' }) }
})

vi.mock('../../../../infrastructure/api/mailboxes/mailboxesApi', () => ({
  getMailboxById: vi.fn().mockResolvedValue(mockMailbox),
  updateMailbox: vi.fn(),
  MailboxType: { WallSmall: 1, StandaloneLarge: 2, IndoorResidential: 3, SpecialPriority: 4 },
  MailboxPriority: { Visok: 1, Srednji: 2, Nizak: 3 },
  mailboxTypeLabels: { 1: 'Zidni (mali)', 2: 'Samostojeći (veliki)', 3: 'Unutrašnji (stambene zgrade)', 4: 'Specijalni (prioritetni)' },
}))

vi.mock('../../../../infrastructure/validation/availabilitySchema', async () => {
  const actual = await vi.importActual('../../../../infrastructure/validation/availabilitySchema') as any;
  return {
    ...actual,
    mapAvailabilityToRequest: vi.fn().mockReturnValue({
      isAlwaysAvailable: false,
      slot1Start: '08:00',
      slot1End: '16:00',
      slot2Start: null,
      slot2End: null,
    }),
  }
})

vi.mock('../../../components/mailboxes/AvailabilitySection', () => ({
  AvailabilitySection: () => <div data-testid="availability-section" />,
}))

vi.mock('../../../components/common/OpenStreetMapPicker', () => ({
  default: ({ onLocationSelect, onAddressFound }: any) => (
    <button type="button" onClick={() => { onLocationSelect(44, 18); onAddressFound('Nova 1'); }}>Mock Map</button>
  ),
}))

vi.mock('../../../components/Layout/Layout', () => ({
  Layout: ({ children }: any) => <div>{children}</div>,
}))

vi.mock('sonner', () => ({
  toast: { success: vi.fn(), error: vi.fn() },
}))

import { getMailboxById, updateMailbox } from '../../../../infrastructure/api/mailboxes/mailboxesApi'
import { toast } from 'sonner'

const renderPage = () => render(<BrowserRouter><EditMailboxPage /></BrowserRouter>)

describe('EditMailboxPage', () => {
  beforeEach(() => {
    vi.clearAllMocks()
    vi.mocked(getMailboxById).mockResolvedValue(mockMailbox as any)
  })

  it('učitava i prikazuje podatke', async () => {
    renderPage()
    await waitFor(() => {
      expect(screen.getByLabelText(/serijski broj/i)).toHaveValue('SN001')
    })
  })

  it('dozvoljava promjenu kapaciteta', async () => {
    const user = userEvent.setup()
    renderPage()
    const input = await screen.findByLabelText(/kapacitet/i)
    await user.clear(input)
    await user.type(input, '200')
    expect(input).toHaveValue(200)
  })

  it('poziva updateMailbox pri slanju', async () => {
    const user = userEvent.setup()
    vi.mocked(updateMailbox).mockResolvedValue({} as any)
    renderPage()
    
    await screen.findByLabelText(/serijski broj/i)
    
    const submitBtn = screen.getByRole('button', { name: /spremi promjene/i })
    await user.click(submitBtn)

    await waitFor(() => {
      expect(updateMailbox).toHaveBeenCalled()
    })
  })

  it('prikazuje toast greške pri neuspjehu', async () => {
    const user = userEvent.setup()
    vi.mocked(updateMailbox).mockRejectedValue(new Error('Greška'))
    renderPage()
    
    await screen.findByLabelText(/serijski broj/i)
    await user.click(screen.getByRole('button', { name: /spremi promjene/i }))

    await waitFor(() => {
      expect(toast.error).toHaveBeenCalled()
    })
  })
})