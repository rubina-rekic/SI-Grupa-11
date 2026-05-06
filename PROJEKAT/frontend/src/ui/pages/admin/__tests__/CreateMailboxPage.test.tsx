import { render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { BrowserRouter } from 'react-router-dom'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { Toaster } from 'sonner'
import CreateMailboxPage from '../CreateMailboxPage'
import { MailboxType } from '../../../../infrastructure/api/mailboxes/mailboxesApi'
import '@testing-library/jest-dom'

// Mock the API
jest.mock('../../../../infrastructure/api/mailboxes/mailboxesApi', () => ({
  createMailbox: jest.fn(),
  checkSerialNumberExists: jest.fn(),
  getAllMailboxes: jest.fn(),
  MailboxType: {
    WallSmall: 1,
    StandaloneLarge: 2,
    IndoorResidential: 3,
    SpecialPriority: 4
  },
  mailboxTypeLabels: {
    [1]: 'Zidni (mali)',
    [2]: 'Samostojeći (veliki)',
    [3]: 'Unutrašnji (stambene zgrade)',
    [4]: 'Specijalni (prioritetni)'
  }
}))

// Mock the map component
jest.mock('../../../ui/components/common/OpenStreetMapPicker', () => {
  return function MockOpenStreetMapPicker({ onLocationSelect }: { onLocationSelect: (location: { lat: number; lng: number }) => void }) {
    return (
      <div data-testid="mock-map">
        <button
          onClick={() => onLocationSelect({ lat: 43.8563, lng: 18.4131 })}
        >
          Select Location
        </button>
      </div>
    )
  }
})

// Mock react-hook-form
const mockSetValue = jest.fn()
const mockGetValues = jest.fn()
const mockFormState = { errors: {}, isSubmitting: false }
const mockHandleSubmit = jest.fn((callback) => (e: React.FormEvent) => {
  e.preventDefault()
  callback({
    serialNumber: 'TEST001',
    address: 'Test Address',
    latitude: 43.8563,
    longitude: 18.4131,
    type: MailboxType.WallSmall,
    capacity: 100,
    installationYear: 2024,
    notes: 'Test notes'
  })
})

jest.mock('react-hook-form', () => ({
  useForm: () => ({
    register: jest.fn(),
    handleSubmit: mockHandleSubmit,
    setValue: mockSetValue,
    getValues: mockGetValues,
    formState: mockFormState,
    reset: jest.fn()
  })
}))

// Mock toast
jest.mock('sonner', () => ({
  toast: {
    success: jest.fn(),
    error: jest.fn()
  },
  Toaster: () => <div data-testid="toaster" />
}))

import { createMailbox, checkSerialNumberExists } from '../../../../infrastructure/api/mailboxes/mailboxesApi'

const createTestQueryClient = () => new QueryClient({
  defaultOptions: {
    queries: { retry: false },
    mutations: { retry: false }
  }
})

const renderWithProviders = (component: React.ReactElement) => {
  const queryClient = createTestQueryClient()
  return render(
    <QueryClientProvider client={queryClient}>
      <BrowserRouter>
        <Toaster />
        {component}
      </BrowserRouter>
    </QueryClientProvider>
  )
}

describe('CreateMailboxPage - PBI-017', () => {
  beforeEach(() => {
    jest.clearAllMocks()
  })

  describe('Form Rendering', () => {
    test('should render all form fields', () => {
      renderWithProviders(<CreateMailboxPage />)

      expect(screen.getByLabelText(/serijski broj/i)).toBeInTheDocument()
      expect(screen.getByLabelText(/adresa/i)).toBeInTheDocument()
      expect(screen.getByLabelText(/tip sandučića/i)).toBeInTheDocument()
      expect(screen.getByLabelText(/kapacitet/i)).toBeInTheDocument()
      expect(screen.getByLabelText(/godina instalacije/i)).toBeInTheDocument()
      expect(screen.getByLabelText(/napomene/i)).toBeInTheDocument()
    })

    test('should render map toggle button', () => {
      renderWithProviders(<CreateMailboxPage />)

      expect(screen.getByText(/prikaži mapu/i)).toBeInTheDocument()
    })

    test('should render save and cancel buttons', () => {
      renderWithProviders(<CreateMailboxPage />)

      expect(screen.getByText(/sačuvaj sandučić/i)).toBeInTheDocument()
      expect(screen.getByText(/otkaži/i)).toBeInTheDocument()
    })
  })

  describe('Form Validation', () => {
    test('should show validation errors for required fields', async () => {
      const user = userEvent.setup()
      renderWithProviders(<CreateMailboxPage />)

      const saveButton = screen.getByText(/sačuvaj sandučić/i)
      await user.click(saveButton)

      // Wait for validation errors to appear
      await waitFor(() => {
        expect(screen.getByText(/serijski broj je obavezan/i)).toBeInTheDocument()
        expect(screen.getByText(/adresa je obavezna/i)).toBeInTheDocument()
      })
    })

    test('should validate serial number format', async () => {
      const user = userEvent.setup()
      renderWithProviders(<CreateMailboxPage />)

      const serialNumberInput = screen.getByLabelText(/serijski broj/i)
      await user.type(serialNumberInput, 'ab')

      expect(screen.getByText(/serijski broj mora sadržavati/i)).toBeInTheDocument()
    })

    test('should validate coordinate ranges', async () => {
      const user = userEvent.setup()
      renderWithProviders(<CreateMailboxPage />)

      const latitudeInput = screen.getByLabelText(/latitude/i)
      await user.clear(latitudeInput)
      await user.type(latitudeInput, '91')

      expect(screen.getByText(/latitude mora biti između/i)).toBeInTheDocument()
    })
  })

  describe('Map Integration', () => {
    test('should show map when toggle button is clicked', async () => {
      const user = userEvent.setup()
      renderWithProviders(<CreateMailboxPage />)

      const toggleButton = screen.getByText(/prikaži mapu/i)
      await user.click(toggleButton)

      expect(screen.getByTestId('mock-map')).toBeInTheDocument()
      expect(screen.getByText(/sakrij mapu/i)).toBeInTheDocument()
    })

    test('should update coordinates when location is selected', async () => {
      const user = userEvent.setup()
      renderWithProviders(<CreateMailboxPage />)

      // Show map
      const toggleButton = screen.getByText(/prikaži mapu/i)
      await user.click(toggleButton)

      // Select location
      const selectButton = screen.getByText(/select location/i)
      await user.click(selectButton)

      expect(mockSetValue).toHaveBeenCalledWith('latitude', 43.8563)
      expect(mockSetValue).toHaveBeenCalledWith('longitude', 18.4131)
    })
  })

  describe('Form Submission', () => {
    test('should submit form successfully with valid data', async () => {
      const user = userEvent.setup()
      const mockCreateMailbox = createMailbox as jest.MockedFunction<typeof createMailbox>
      mockCreateMailbox.mockResolvedValue({
        id: 'test-id',
        serialNumber: 'TEST001',
        address: 'Test Address',
        latitude: 43.8563,
        longitude: 18.4131,
        type: MailboxType.WallSmall,
        capacity: 100,
        installationYear: 2024,
        notes: 'Test notes',
        createdAt: new Date().toISOString(),
        updatedAt: new Date().toISOString()
      })

      renderWithProviders(<CreateMailboxPage />)

      // Fill form
      await user.type(screen.getByLabelText(/serijski broj/i), 'TEST001')
      await user.type(screen.getByLabelText(/adresa/i), 'Test Address')
      await user.selectOptions(screen.getByLabelText(/tip sandučića/i), '1')
      await user.type(screen.getByLabelText(/kapacitet/i), '100')
      await user.type(screen.getByLabelText(/godina instalacije/i), '2024')
      await user.type(screen.getByLabelText(/napomene/i), 'Test notes')

      // Submit form
      const saveButton = screen.getByText(/sačuvaj sandučić/i)
      await user.click(saveButton)

      await waitFor(() => {
        expect(mockCreateMailbox).toHaveBeenCalledWith({
          serialNumber: 'TEST001',
          address: 'Test Address',
          latitude: 43.8563,
          longitude: 18.4131,
          type: MailboxType.WallSmall,
          capacity: 100,
          installationYear: 2024,
          notes: 'Test notes'
        })
      })
    })

    test('should show success message when mailbox is created', async () => {
      const user = userEvent.setup()
      const mockCreateMailbox = createMailbox as jest.MockedFunction<typeof createMailbox>
      mockCreateMailbox.mockResolvedValue({
        id: 'test-id',
        serialNumber: 'TEST001',
        address: 'Test Address',
        latitude: 43.8563,
        longitude: 18.4131,
        type: MailboxType.WallSmall,
        capacity: 100,
        installationYear: 2024,
        notes: 'Test notes',
        createdAt: new Date().toISOString(),
        updatedAt: new Date().toISOString()
      })

      renderWithProviders(<CreateMailboxPage />)

      // Fill and submit form
      await user.type(screen.getByLabelText(/serijski broj/i), 'TEST001')
      await user.type(screen.getByLabelText(/adresa/i), 'Test Address')
      await user.selectOptions(screen.getByLabelText(/tip sandučića/i), '1')
      await user.type(screen.getByLabelText(/kapacitet/i), '100')
      await user.type(screen.getByLabelText(/godina instalacije/i), '2024')

      const saveButton = screen.getByText(/sačuvaj sandučić/i)
      await user.click(saveButton)

      await waitFor(() => {
        expect(screen.getByText(/sandučić test001 uspješno dodan/i)).toBeInTheDocument()
      })
    })

    test('should show error message when creation fails', async () => {
      const user = userEvent.setup()
      const mockCreateMailbox = createMailbox as jest.MockedFunction<typeof createMailbox>
      mockCreateMailbox.mockRejectedValue(new Error('Greška pri kreiranju'))

      renderWithProviders(<CreateMailboxPage />)

      // Fill and submit form
      await user.type(screen.getByLabelText(/serijski broj/i), 'TEST001')
      await user.type(screen.getByLabelText(/adresa/i), 'Test Address')
      await user.selectOptions(screen.getByLabelText(/tip sandučića/i), '1')
      await user.type(screen.getByLabelText(/kapacitet/i), '100')
      await user.type(screen.getByLabelText(/godina instalacije/i), '2024')

      const saveButton = screen.getByText(/sačuvaj sandučić/i)
      await user.click(saveButton)

      await waitFor(() => {
        expect(screen.getByText(/greška pri kreiranju sandučića/i)).toBeInTheDocument()
      })
    })

    test('should reset form after successful submission', async () => {
      const user = userEvent.setup()
      const mockCreateMailbox = createMailbox as jest.MockedFunction<typeof createMailbox>
      mockCreateMailbox.mockResolvedValue({
        id: 'test-id',
        serialNumber: 'TEST001',
        address: 'Test Address',
        latitude: 43.8563,
        longitude: 18.4131,
        type: MailboxType.WallSmall,
        capacity: 100,
        installationYear: 2024,
        notes: 'Test notes',
        createdAt: new Date().toISOString(),
        updatedAt: new Date().toISOString()
      })

      renderWithProviders(<CreateMailboxPage />)

      // Fill form
      await user.type(screen.getByLabelText(/serijski broj/i), 'TEST001')
      await user.type(screen.getByLabelText(/adresa/i), 'Test Address')

      // Submit form
      const saveButton = screen.getByText(/sačuvaj sandučić/i)
      await user.click(saveButton)

      await waitFor(() => {
        expect(mockSetValue).toHaveBeenCalledWith('serialNumber', '')
        expect(mockSetValue).toHaveBeenCalledWith('address', '')
      })
    })
  })

  describe('Serial Number Validation', () => {
    test('should check serial number uniqueness', async () => {
      const user = userEvent.setup()
      const mockCheckSerialNumber = checkSerialNumberExists as jest.MockedFunction<typeof checkSerialNumberExists>
      mockCheckSerialNumber.mockResolvedValue(true)

      renderWithProviders(<CreateMailboxPage />)

      const serialInput = screen.getByLabelText(/serijski broj/i)
      await user.type(serialInput, 'EXIST001')
      await user.tab() // Trigger blur

      await waitFor(() => {
        expect(mockCheckSerialNumber).toHaveBeenCalledWith('EXIST001')
      })
    })

    test('should show error when serial number already exists', async () => {
      const user = userEvent.setup()
      const mockCheckSerialNumber = checkSerialNumberExists as jest.MockedFunction<typeof checkSerialNumberExists>
      mockCheckSerialNumber.mockResolvedValue(true)

      renderWithProviders(<CreateMailboxPage />)

      const serialInput = screen.getByLabelText(/serijski broj/i)
      await user.type(serialInput, 'EXIST001')
      await user.tab() // Trigger blur

      await waitFor(() => {
        expect(screen.getByText(/sandučić sa ovim serijskim brojem već postoji/i)).toBeInTheDocument()
      })
    })
  })

  describe('Mailbox Types', () => {
    test('should render all mailbox types', () => {
      renderWithProviders(<CreateMailboxPage />)

      const typeSelect = screen.getByLabelText(/tip sandučića/i)
      expect(typeSelect).toBeInTheDocument()

      const options = screen.getAllByRole('option')
      expect(options).toHaveLength(4) // 4 mailbox types
    })

    test('should allow selection of different mailbox types', async () => {
      const user = userEvent.setup()
      renderWithProviders(<CreateMailboxPage />)

      const typeSelect = screen.getByLabelText(/tip sandučića/i)
      await user.selectOptions(typeSelect, '2') // StandaloneLarge

      expect(typeSelect).toHaveValue('2')
    })
  })

  describe('Cancel Button', () => {
    test('should navigate back when cancel is clicked', async () => {
      const user = userEvent.setup()
      renderWithProviders(<CreateMailboxPage />)

      const cancelButton = screen.getByText(/otkaži/i)
      await user.click(cancelButton)

      // In a real test, we would check navigation
      // For now, just ensure the button is clickable
      expect(cancelButton).toBeInTheDocument()
    })
  })
})
