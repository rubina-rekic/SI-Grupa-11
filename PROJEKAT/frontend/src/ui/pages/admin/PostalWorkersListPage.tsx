import { useCallback, useEffect, useState } from "react"
import { Layout } from "../../components/Layout/Layout"
import { getUsers, type UserListDto } from "../../../infrastructure/api/users/usersApi"
import { useNavigate } from "react-router-dom"

export default function PostalWorkersListPage() {
  const [users, setUsers] = useState<UserListDto[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [statusSorted, setStatusSorted] = useState(false)
  const navigate = useNavigate()

  const sortUsersByStatus = (items: UserListDto[]) => {
    return [...items].sort((a, b) => {
      const aActive = !a.isLockedOut
      const bActive = !b.isLockedOut

      if (aActive === bActive) {
        return a.username.localeCompare(b.username)
      }

      return aActive ? -1 : 1
    })
  }

  const fetchUsers = useCallback(async () => {
    setLoading(true)
    setError(null)

    const result = await getUsers()
    if (result.data) {
      const postalWorkers = result.data.filter(user => user.role === "PostalWorker")
      setUsers(statusSorted ? sortUsersByStatus(postalWorkers) : postalWorkers)
    } else {
      setError("Greška pri učitavanju korisnika.")
    }
    setLoading(false)
  }, [statusSorted])

  useEffect(() => {
    fetchUsers()
  }, [fetchUsers])

  const handleStatusHeaderClick = () => {
    setUsers(prevUsers => sortUsersByStatus(prevUsers))
    setStatusSorted(true)
  }

  const handleRefreshClick = async () => {
    await fetchUsers()
  }

  const getStatusColor = (isLockedOut: boolean) => {
    return isLockedOut ? "#e74c3c" : "#27ae60"
  }

  const getStatusLabel = (isLockedOut: boolean) => {
    return isLockedOut ? "Zaključan" : "Aktivan"
  }

  if (loading) return <Layout><div className="page-container">Učitavanje...</div></Layout>
  if (error) return <Layout><div className="page-container">{error}</div></Layout>

  return (
    <Layout>
      <div className="page-container">
        <div className="form-card">
          <div className="form-card__header" style={{ display: "flex", justifyContent: "space-between", flexWrap: "wrap", gap: "0.75rem" }}>
            <h1 className="form-card__title">Lista korisnika</h1>
            <div style={{ display: "flex", gap: "0.75rem", flexWrap: "wrap" }}>
              <button className="btn btn--primary" onClick={() => navigate("/admin/users/new") }>
                + Dodaj korisnika
              </button>
              <button className="btn btn--secondary" onClick={handleRefreshClick} disabled={loading}>
                Osvježi
              </button>
            </div>
          </div>

          {users.length === 0 ? (
            <p>Nema registrovanih korisnika.</p>
          ) : (
            <table style={{ width: "100%", borderCollapse: "collapse", marginTop: "1rem" }}>
              <thead>
                <tr style={{ borderBottom: "2px solid #e0e0e0", textAlign: "left" }}>
                  <th style={{ padding: "12px" }}>Korisničko ime</th>
                  <th style={{ padding: "12px" }}>Email</th>
                  <th style={{ padding: "12px" }}>Uloga</th>
                  <th style={{ padding: "12px", cursor: "pointer" }} onClick={handleStatusHeaderClick}>
                    Status
                  </th>
                </tr>
              </thead>
              <tbody>
                {users.map((user) => (
                  <tr key={user.id} style={{ borderBottom: "1px solid #f0f0f0" }}>
                    <td style={{ padding: "12px" }}>{user.username}</td>
                    <td style={{ padding: "12px" }}>{user.email}</td>
                    <td style={{ padding: "12px" }}>
                      Poštar
                    </td>
                    <td style={{ padding: "12px" }}>
                      <span style={{
                        display: "inline-flex",
                        alignItems: "center",
                        gap: "6px"
                      }}>
                        <span style={{
                          width: "10px",
                          height: "10px",
                          borderRadius: "50%",
                          backgroundColor: getStatusColor(user.isLockedOut),
                          display: "inline-block"
                        }} />
                        {getStatusLabel(user.isLockedOut)}
                      </span>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          )}
        </div>
      </div>
    </Layout>
  )
}