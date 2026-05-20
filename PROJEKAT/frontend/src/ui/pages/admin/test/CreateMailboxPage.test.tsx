import { render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { vi, describe, it, expect, beforeEach } from 'vitest'
import { BrowserRouter } from 'react-router-dom'
import CreateMailboxPage from '../CreateMailboxPage'

const mockNavigate = vi.fn()
vi.mock('react-router-dom', async () => {
  const actual = await vi.importActual('react-router-dom')
  return { ...actual, useNavigate: () => mockNavigate }
})

vi.mock('../../../../infrastructure/api/mailboxes/mailboxesApi', () => ({
  createMailbox: vi.fn(),
  checkSerialNumberExists: vi.fn().mockResolvedValue(false),
  MailboxType: { WallSmall: 1, StandaloneLarge: 2, IndoorResidential: 3, SpecialPriority: 4 },
  MailboxPriority: { Visok: 1, Srednji: 2, Nizak: 3 },
  MailboxStatus: { Prazan: 0, Pun: 1 },
  MailboxWorkingDays: {
    None: 0,
    Ponedjeljak: 1,
    Utorak: 2,
    Srijeda: 4,
    Cetvrtak: 8,
    Petak: 16,
    Subota: 32,
    Nedjelja: 64,
    RadniDani: 31,
    Vikend: 96,
    SvakiDan: 127,
  },
  workingDayBits: [
    { name: 'Ponedjeljak', bit: 1 },
    { name: 'Utorak', bit: 2 },
    { name: 'Srijeda', bit: 4 },
    { name: 'Cetvrtak', bit: 8 },
    { name: 'Petak', bit: 16 },
    { name: 'Subota', bit: 32 },
    { name: 'Nedjelja', bit: 64 },
  ],
  mailboxTypeLabels: {
    1: 'Zidni (mali)',
    2: 'Samostojeći (veliki)',
    3: 'Unutrašnji (stambene zgrade)',
    4: 'Specijalni (prioritetni)',
  },
}))

vi.mock('../../../../infrastructure/validation/availabilitySchema', async () => {
  const { z } = await import('zod')
  return {
    availabilitySchema: z.object({
      isAlwaysAvailable: z.boolean().optional(),
      hasSecondSlot: z.boolean().optional(),
      slot1Start: z.string().optional(),
      slot1End: z.string().optional(),
      slot2Start: z.string().optional(),
      slot2End: z.string().optional(),
    }),
    mapAvailabilityToRequest: vi.fn().mockReturnValue({
      isAlwaysAvailable: false,
      slot1Start: null,
      slot1End: null,
      slot2Start: null,
      slot2End: null,
    }),
  }
})

vi.mock('../../../components/mailboxes/AvailabilitySection', () => ({
  AvailabilitySection: () => <div data-testid="availability-section" />,
}))

vi.mock('../../../components/common/OpenStreetMapPicker', () => ({
  default: ({
    onLocationSelect,
    onAddressFound,
  }: {
    onLocationSelect: (lat: number, lng: number) => void
    onAddressFound: (address: string) => void
  }) => (
    <div data-testid="map-picker">
      <button
        type="button"
        data-testid="select-location-btn"
        onClick={() => {
          onLocationSelect(43.8563, 18.4131)
          onAddressFound('Titova 1, Sarajevo')
        }}
      >
        Odaberi lokaciju
      </button>
    </div>
  ),
}))

vi.mock('../../../components/Layout/Layout', () => ({
  Layout: ({ children }: { children: React.ReactNode }) => <div>{children}</div>,
}))

vi.mock('sonner', () => ({
  toast: { success: vi.fn(), error: vi.fn() },
}))

import { createMailbox, checkSerialNumberExists, type MailboxResponse } from '../../../../infrastructure/api/mailboxes/mailboxesApi'
import { toast } from 'sonner'

const renderPage = () =>
  render(
    <BrowserRouter>
      <CreateMailboxPage />
    </BrowserRouter>
  )

describe('CreateMailboxPage — PBI-017', () => {
  beforeEach(() => {
    vi.clearAllMocks()
    vi.mocked(checkSerialNumberExists).mockResolvedValue(false)
  })

  describe('Renderovanje forme', () => {
    it('prikazuje polje za serijski broj', () => {
      renderPage()
      expect(screen.getByLabelText(/serijski broj/i)).toBeInTheDocument()
    })

    it('prikazuje dropdown za tip sandučića', () => {
      renderPage()
      expect(screen.getByLabelText(/tip sandučića/i)).toBeInTheDocument()
    })

    it('prikazuje dropdown za prioritet', () => {
      renderPage()
      expect(screen.getByLabelText(/prioritet/i)).toBeInTheDocument()
    })

    it('prikazuje polje za kapacitet', () => {
      renderPage()
      expect(screen.getByLabelText(/kapacitet/i)).toBeInTheDocument()
    })

    it('prikazuje polje za godinu instalacije', () => {
      renderPage()
      expect(screen.getByLabelText(/godina instalacije/i)).toBeInTheDocument()
    })

    it('prikazuje polje za napomene', () => {
      renderPage()
      expect(screen.getByLabelText(/napomene/i)).toBeInTheDocument()
    })

    it('prikazuje komponentu mape', () => {
      renderPage()
      expect(screen.getByTestId('map-picker')).toBeInTheDocument()
    })

    it('prikazuje sekciju dostupnosti', () => {
      renderPage()
      expect(screen.getByTestId('availability-section')).toBeInTheDocument()
    })

    it('prikazuje dugme za čuvanje i otkazivanje', () => {
      renderPage()
      expect(screen.getByRole('button', { name: /sačuvaj sandučić/i })).toBeInTheDocument()
      expect(screen.getByRole('button', { name: /otkaži/i })).toBeInTheDocument()
    })

    it('dugme za čuvanje je onemogućeno dok lokacija nije odabrana', () => {
      renderPage()
      expect(screen.getByRole('button', { name: /sačuvaj sandučić/i })).toBeDisabled()
    })

    it('prikazuje upozorenje da lokacija nije odabrana', () => {
      renderPage()
      expect(screen.getByText(/lokacija nije odabrana/i)).toBeInTheDocument()
    })

    it('prikazuje sva 4 tipa sandučića u dropdownu', () => {
      renderPage()
      const options = screen.getByLabelText(/tip sandučića/i).querySelectorAll('option')
      expect(options).toHaveLength(4)
    })
  })

  describe('Odabir lokacije na mapi', () => {
    it('aktivira dugme za čuvanje nakon odabira lokacije', async () => {
      const user = userEvent.setup()
      renderPage()
      await user.click(screen.getByTestId('select-location-btn'))
      expect(screen.getByRole('button', { name: /sačuvaj sandučić/i })).not.toBeDisabled()
    })

    it('prikazuje koordinate odabrane lokacije', async () => {
      const user = userEvent.setup()
      renderPage()
      await user.click(screen.getByTestId('select-location-btn'))
      expect(screen.getByText(/odabrana lokacija/i)).toBeInTheDocument()
    })

    it('Å¡alje adresu pronaÄ‘enu na mapi', async () => {
      const user = userEvent.setup()
      vi.mocked(createMailbox).mockResolvedValue({} as MailboxResponse)
      renderPage()
      await user.click(screen.getByTestId('select-location-btn'))
      await user.type(screen.getByLabelText(/serijski broj/i), 'SN001')
      await user.click(screen.getByRole('button', { name: /sačuvaj sandučić/i }))

      await waitFor(() => {
        expect(createMailbox).toHaveBeenCalledWith(expect.objectContaining({ address: 'Titova 1, Sarajevo' }))
      })
    })
  })

  describe('Provjera serijskog broja', () => {
    it('poziva API za provjeru serijskog broja', async () => {
      const user = userEvent.setup()
      renderPage()
      await user.type(screen.getByLabelText(/serijski broj/i), 'SN001')
      await user.tab()
      await waitFor(() => {
        expect(checkSerialNumberExists).toHaveBeenCalledWith('SN001')
      })
    })

    it('prikazuje grešku ako serijski broj već postoji', async () => {
      const user = userEvent.setup()
      vi.mocked(checkSerialNumberExists).mockResolvedValue(true)
      renderPage()
      await user.type(screen.getByLabelText(/serijski broj/i), 'EXIST001')
      await user.tab()
      await waitFor(() => {
        expect(
          screen.getByText(/sandučić sa ovim serijskim brojem već postoji/i)
        ).toBeInTheDocument()
      })
    })
  })

  describe('Tipovi sandučića', () => {
    it('dozvoljava odabir različitih tipova sandučića', async () => {
      const user = userEvent.setup()
      renderPage()
      const typeSelect = screen.getByLabelText(/tip sandučića/i)
      await user.selectOptions(typeSelect, '2')
      expect((typeSelect as HTMLSelectElement).value).toBe('2')
    })
  })

  describe('Slanje forme', () => {
    it('uspješno šalje formu s validnim podacima', async () => {
      const user = userEvent.setup()
      vi.mocked(createMailbox).mockResolvedValue({} as MailboxResponse)
      renderPage()

      await user.click(screen.getByTestId('select-location-btn'))
      await user.type(screen.getByLabelText(/serijski broj/i), 'SN001')
      await user.click(screen.getByRole('button', { name: /sačuvaj sandučić/i }))

      await waitFor(() => {
        expect(createMailbox).toHaveBeenCalledWith(
          expect.objectContaining({
            serialNumber: 'SN001',
            latitude: 43.8563,
            longitude: 18.4131,
          })
        )
      })
    })

    it('prikazuje toast uspjeha nakon kreiranja', async () => {
      const user = userEvent.setup()
      vi.mocked(createMailbox).mockResolvedValue({} as MailboxResponse)
      renderPage()

      await user.click(screen.getByTestId('select-location-btn'))
      await user.type(screen.getByLabelText(/serijski broj/i), 'SN001')
      await user.click(screen.getByRole('button', { name: /sačuvaj sandučić/i }))

      await waitFor(() => {
        expect(toast.success).toHaveBeenCalledWith(expect.stringContaining('SN001'))
      })
    })

    it('navigira na listu sandučića nakon uspješnog kreiranja', async () => {
      const user = userEvent.setup()
      vi.mocked(createMailbox).mockResolvedValue({} as MailboxResponse)
      renderPage()

      await user.click(screen.getByTestId('select-location-btn'))
      await user.type(screen.getByLabelText(/serijski broj/i), 'SN001')
      await user.click(screen.getByRole('button', { name: /sačuvaj sandučić/i }))

      await waitFor(() => {
        expect(mockNavigate).toHaveBeenCalledWith('/admin/mailboxes')
      })
    })

    it('prikazuje toast greške kada API vrati grešku', async () => {
      const user = userEvent.setup()
      vi.mocked(createMailbox).mockRejectedValue(new Error('Server error'))
      renderPage()

      await user.click(screen.getByTestId('select-location-btn'))
      await user.type(screen.getByLabelText(/serijski broj/i), 'SN001')
      await user.click(screen.getByRole('button', { name: /sačuvaj sandučić/i }))

      await waitFor(() => {
        expect(toast.error).toHaveBeenCalledWith(
          expect.stringContaining('Greška pri kreiranju sandučića')
        )
      })
    })
  })

  describe('Otkazivanje', () => {
    it('navigira nazad pri kliku na Otkaži', async () => {
      const user = userEvent.setup()
      renderPage()
      await user.click(screen.getByRole('button', { name: /otkaži/i }))
      expect(mockNavigate).toHaveBeenCalledWith('/admin/mailboxes')
    })
  })
})
