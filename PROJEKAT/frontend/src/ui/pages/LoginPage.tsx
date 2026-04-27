import '../../styles/global.css';
import { useState } from 'react';

export default function LoginPage() {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState('');
    const [isLoading, setIsLoading] = useState(false);
    const [isLocked, setIsLocked] = useState(false);

    const handleLogin = async (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        
        if (isLocked) {
            setError('Account locked');
            return;
        }

        setIsLoading(true);
        setError('');

        try {
            const response = await fetch('http://localhost:5032/api/users/login', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({ email, password }),
            });

            if (response.ok) {
                const data = await response.json();
                localStorage.setItem('token', data.token);
                window.location.href = '/dashboard';
            } else if (response.status === 423) {
                setIsLocked(true);
                setError('Account locked due to too many failed attempts.');
            } else {
                setError('Invalid credentials.');
            }
        } catch (err) {
            setError('Connection error. Please try again.');
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
                            className="form-field__input"
                            placeholder="npr. amar.hodzic@posta.ba"
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
                            disabled={isLocked}
                            required
                        />
                    </div>

                    <div className="form-field">
                        <label className="form-field__label" htmlFor="password">
                            Lozinka
                        </label>
                        <input
                            id="password"
                            type="password"
                            className="form-field__input"
                            placeholder="Unesite lozinku"
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                            disabled={isLocked}
                            required
                        />
                    </div>

                    <div className="form-actions">
                        <button
                            type="submit"
                            className="btn btn--primary"
                            disabled={isLoading || isLocked}
                        >
                            {isLoading ? 'Prijavljivanje…' : 'Prijavi se'}
                        </button>
                    </div>
                </form>
            </div>
        </div>
    );
}
