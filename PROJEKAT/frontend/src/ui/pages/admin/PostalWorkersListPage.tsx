import { useEffect, useState } from "react"
import { Layout } from "../../components/Layout/Layout"
import { getUsers, type UserListDto } from "../../../infrastructure/api/users/usersApi"
import { useNavigate } from "react-router-dom"

export default function PostalWorkersListPage() {
  const [users, setUsers] = useState<UserListDto[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const navigate = useNavigate()

  useEffect(() => {
    const fetchUsers = async () => {
      const result = await getUsers()
      if (result.data) {
        setUsers(result.data.filter(user => user.role === "PostalWorker"))
      } else {
        setError("Greška pri učitavanju korisnika.")
      }
      setLoading(false)
    }
    fetchUsers()
  }, [])

  const getStatusColor = (role: string) => {
    return role === "Administrator" ? "#e74c3c" : "#27ae60"
  }

  if (loading) return <Layout><div className="page-container">Učitavanje...</div></Layout>
  if (error) return <Layout><div className="page-container">{error}</div></Layout>

  return (
    <Layout>
      <div className="page-container">
        <div className="form-card">
          <div className="form-card__header">
            <h1 className="form-card__title">Lista korisnika</h1>
            <button className="btn btn--primary" onClick={() => navigate("/admin/users/new")}>
              + Dodaj korisnika
            </button>
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
                  <th style={{ padding: "12px" }}>Status</th>
                </tr>
              </thead>
              <tbody>
                {users.map((user) => (
                  <tr key={user.id} style={{ borderBottom: "1px solid #f0f0f0" }}>
                    <td style={{ padding: "12px" }}>{user.username}</td>
                    <td style={{ padding: "12px" }}>{user.email}</td>
                    <td style={{ padding: "12px" }}>
                      {user.role === "Administrator" ? "Administrator" : "Poštar"}
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
                          backgroundColor: getStatusColor(user.role),
                          display: "inline-block"
                        }} />
                        {user.role === "Administrator" ? "Administrator" : "Aktivan"}
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