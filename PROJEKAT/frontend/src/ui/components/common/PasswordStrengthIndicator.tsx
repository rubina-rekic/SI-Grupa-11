interface Props {
  password: string
}

type StrengthLevel = "weak" | "fair" | "good" | "strong"

interface Strength {
  score: number
  label: string
  level: StrengthLevel | null
}

function calcStrength(password: string): Strength {
  if (!password) return { score: 0, label: "", level: null }

  let score = 0
  if (password.length >= 8) score++
  if (/[A-Z]/.test(password)) score++
  if (/\d/.test(password)) score++
  if (password.length >= 12) score++

  if (score === 1) return { score, label: "Slaba", level: "weak" }
  if (score === 2) return { score, label: "Srednja", level: "fair" }
  if (score === 3) return { score, label: "Dobra", level: "good" }
  return { score, label: "Jaka", level: "strong" }
}

export function PasswordStrengthIndicator({ password }: Props) {
  const { score, label, level } = calcStrength(password)

  if (!password) return null

  return (
    <div className="password-strength">
      <div className="password-strength__bars">
        {[1, 2, 3, 4].map((i) => (
          <div
            key={i}
            className={`password-strength__bar${i <= score && level ? ` password-strength__bar--${level}` : ""}`}
          />
        ))}
      </div>
      <span className={`password-strength__label${level ? ` password-strength__label--${level}` : ""}`}>
        {label}
      </span>
    </div>
  )
}
