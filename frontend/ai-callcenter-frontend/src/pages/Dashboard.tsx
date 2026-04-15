import { useEffect, useState } from "react";

interface Complaint {
  id: number;
  ticketId: string;
  userName: string;
  callerPhone: string;
  address: string;
  wardId: number;
  areaId: number;

  category: string;
  description: string;
  department: string;

  createdAt: string;
  status: string;

  assignedTo: string;
  escalationLevel: number;

  stageDueAt: string;

  resolvedAt?: string;
}

function Dashboard() {
  const [complaints, setComplaints] = useState<Complaint[]>([]);

  useEffect(() => {
    fetch("http://localhost:5196/api/complaints")
      .then((res) => res.json())
      .then((data) => setComplaints(data))
      .catch((err) => console.error(err));
  }, []);

  const handleResolve = async (id: number) => {
    try {
      const res = await fetch(
        `http://localhost:5196/api/complaints/resolve/${id}`,
        { method: "PUT" }
      );

      if (!res.ok) {
        alert("Failed to resolve complaint");
        return;
      }

      const updatedComplaint = await res.json();

      setComplaints((prev) =>
        prev.map((c) =>
          c.id === id
            ? { ...c, status: "Resolved", resolvedAt: updatedComplaint.resolvedAt }
            : c
        )
      );
    } catch (error) {
      console.error("Error:", error);
    }
  };

  return (
    <div style={{ padding: "20px" }}>
      <h2>📊 Complaints Dashboard</h2>

      <div style={{ overflowX: "auto" }}>
        <table
          style={{
            width: "100%",
            borderCollapse: "collapse",
            minWidth: "1400px",
          }}
        >
          <thead>
            <tr>
              {[
                "Ticket",
                "User",
                "Phone",
                "Ward",
                "Area",
                "Address",
                "Category",
                "Department",
                "Description",
                "Status",
                "Escalation",
                "Assigned To",
                "Created",
                "Due Time",
                "Resolved At",
                "Action",
              ].map((header) => (
                <th
                  key={header}
                  style={{
                    border: "1px solid #ccc",   // ✅ HEADER BORDER
                    padding: "8px",
                    backgroundColor: "#f5f5f5",
                    whiteSpace: "nowrap",
                  }}
                >
                  {header}
                </th>
              ))}
            </tr>
          </thead>

          <tbody>
            {complaints.map((c) => (
              <tr key={c.id}>
                {[
                  c.ticketId,
                  c.userName,
                  c.callerPhone,
                  c.wardId,
                  c.areaId,
                  c.address,
                  c.category,
                  c.department,
                  c.description,
                ].map((value, index) => (
                  <td
                    key={index}
                    style={{
                      border: "1px solid #ccc",   // ✅ CELL BORDER
                      padding: "8px",
                      whiteSpace: "nowrap",
                    }}
                  >
                    {value}
                  </td>
                ))}

                {/* STATUS */}
                <td
                  style={{
                    border: "1px solid #ccc",
                    padding: "8px",
                    color: c.status === "Resolved" ? "green" : "red",
                    fontWeight: "bold",
                    whiteSpace: "nowrap",
                  }}
                >
                  {c.status}
                </td>

                <td style={{ border: "1px solid #ccc", padding: "8px" }}>
                  Level {c.escalationLevel}
                </td>

                <td style={{ border: "1px solid #ccc", padding: "8px" }}>
                  {c.assignedTo}
                </td>

                <td style={{ border: "1px solid #ccc", padding: "8px" }}>
                  {new Date(c.createdAt).toLocaleString()}
                </td>

                <td style={{ border: "1px solid #ccc", padding: "8px" }}>
                  {new Date(c.stageDueAt).toLocaleString()}
                </td>

                <td style={{ border: "1px solid #ccc", padding: "8px" }}>
                  {c.resolvedAt
                    ? new Date(c.resolvedAt).toLocaleString()
                    : "-"}
                </td>

                {/* ACTION */}
                <td style={{ border: "1px solid #ccc", padding: "8px" }}>
                  {c.status !== "Resolved" ? (
                    <button
                      onClick={() => handleResolve(c.id)}
                      style={{
                        padding: "5px 10px",
                        backgroundColor: "#28a745",
                        color: "white",
                        border: "none",
                        cursor: "pointer",
                        borderRadius: "4px",
                      }}
                    >
                      Resolve
                    </button>
                  ) : (
                    "✔"
                  )}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
}

export default Dashboard;