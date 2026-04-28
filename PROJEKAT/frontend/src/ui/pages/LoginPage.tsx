import '../../styles/global.css';
import { useState } from 'react';
import { useAuth } from '../../application/hooks/useAuth';

export default function LoginPage() {
    const { login } = useAuth();
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState('');
    const [emailError, setEmailError] = useState(false);
    const [passwordError, setPasswordError] = useState(false);
    const [isLoading, setIsLoading] = useState(false);
    const [isLocked, setIsLocked] = useState(false);
    const [cooldown, setCooldown] = useState(false);

    const startCooldown = () => {
        setCooldown(true);

        setTimeout(() => {
            setCooldown(false);
        }, 3000);
    };

    const handleLogin = async (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();

        setError('');
        setEmailError(false);
        setPasswordError(false);

        if (isLocked) {
            setError('Račun je zaključan.');
            return;
        }

        if (!email.trim()) {
            setEmailError(true);
        }

        if (!password.trim()) {
            setPasswordError(true);
        }

        if (!email.trim() || !password.trim()) {
            return;
        }

        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

        if (!emailRegex.test(email)) {
            setEmailError(true);
            setError('Unesite ispravnu email adresu.');
            return;
        }

        setIsLoading(true);

        try {
            await login(email, password);
        } catch (error: any) {
            if (error.message?.includes('locked')) {
                setIsLocked(true);
                setPassword('');
                setError('Račun je zaključan nakon više neuspješnih pokušaja.');
            } else if (error.message?.includes('deactivated')) {
                setPassword('');
                setError('Vaš račun je deaktiviran. Kontaktirajte administratora.');
                startCooldown();
            } else {
                setPassword('');
                setError('Neispravni kredencijali. Molimo pokušajte ponovo.');
                startCooldown();
            }
        } finally {
            setIsLoading(false);
        }
    };

    return (
        <div className="page-container">
            <div className="form-card">
                <div className="form-card__header">
                    <h1 className="form-card__title">Prijava</h1>
                </div>

                <form className="form-card__body" onSubmit={handleLogin}>
                    {error && <p className="form-field__error">{error}</p>}

                    <div className="form-field">
                        <label className="form-field__label" htmlFor="email">
                            Email adresa
                        </label>

                        <input
                            id="email"
                            type="email"
                            placeholder="npr. amar.hodzic@posta.ba"
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
                            disabled={isLocked}
                            className={`form-field__input ${
                                emailError ? 'input-error' : ''
                            }`}
                        />
                    </div>

                    <div className="form-field">
                        <label className="form-field__label" htmlFor="password">
                            Lozinka
                        </label>

                        <input
                            id="password"
                            type="password"
                            placeholder="Unesite lozinku"
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                            disabled={isLocked}
                            className={`form-field__input ${
                                passwordError ? 'input-error' : ''
                            }`}
                        />
                    </div>

                    <div className="form-actions">
                        <button
                            type="submit"
                            className="btn btn--primary"
                            disabled={isLoading || isLocked || cooldown}
                        >
                            {isLoading
                                ? 'Prijavljivanje...'
                                : cooldown
                                ? 'Sačekajte...'
                                : 'Prijavi se'}
                        </button>
                    </div>
                </form>
            </div>
        </div>
    );
}