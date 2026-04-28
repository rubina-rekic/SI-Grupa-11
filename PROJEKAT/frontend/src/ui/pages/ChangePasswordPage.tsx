import '../../styles/global.css';
import { useState } from 'react';

export default function ChangePasswordPage() {
    const [newPassword, setNewPassword] = useState('');
    const [confirmPassword, setConfirmPassword] = useState('');
    const [error, setError] = useState('');
    const [success, setSuccess] = useState('');
    const [isLoading, setIsLoading] = useState(false);

    const validatePassword = (password: string) => {
        const hasMinLength = password.length >= 8;
        const hasNumber = /\d/.test(password);
        const hasSymbol = /[^A-Za-z0-9]/.test(password);

        return hasMinLength && hasNumber && hasSymbol;
    };

    const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();

        setError('');
        setSuccess('');

        if (!newPassword || !confirmPassword) {
            setError('Sva polja su obavezna.');
            return;
        }

        if (newPassword !== confirmPassword) {
            setError('Lozinke se ne podudaraju.');
            return;
        }

        if (!validatePassword(newPassword)) {
            setError(
                'Lozinka mora imati najmanje 8 karaktera, broj i simbol.'
            );
            return;
        }

        setIsLoading(true);

        try {
            const email = localStorage.getItem('email');

            await fetch('http://localhost:5032/api/users/change-password', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({
                email,
                newPassword,
            }),
        });

            localStorage.removeItem('mustChangePassword');

            setSuccess('Lozinka uspješno promijenjena. Dobrodošli!');

            setTimeout(() => {
                window.location.href = '/dashboard';
            }, 1500);
        } catch {
            setError('Greška pri promjeni lozinke.');
        } finally {
            setIsLoading(false);
        }
    };

    return (
        <div className="page-container">
            <div className="form-card">
                <div className="form-card__header">
                    <h1 className="form-card__title">Promjena lozinke</h1>
                </div>

                <form className="form-card__body" onSubmit={handleSubmit}>
                    {error && <p className="form-field__error">{error}</p>}
                    {success && (
                        <p style={{ color: 'green', fontWeight: 600 }}>
                            {success}
                        </p>
                    )}

                    <div className="form-field">
                        <label className="form-field__label">
                            Nova lozinka
                        </label>

                        <input
                            type="password"
                            className="form-field__input"
                            value={newPassword}
                            onChange={(e) =>
                                setNewPassword(e.target.value)
                            }
                        />
                    </div>

                    <div className="form-field">
                        <label className="form-field__label">
                            Potvrdi lozinku
                        </label>

                        <input
                            type="password"
                            className="form-field__input"
                            value={confirmPassword}
                            onChange={(e) =>
                                setConfirmPassword(e.target.value)
                            }
                        />
                    </div>

                    <div className="form-actions">
                        <button
                            type="submit"
                            className="btn btn--primary"
                            disabled={isLoading}
                        >
                            {isLoading ? 'Spašavanje...' : 'Spasi'}
                        </button>
                    </div>
                </form>
            </div>
        </div>
    );
}