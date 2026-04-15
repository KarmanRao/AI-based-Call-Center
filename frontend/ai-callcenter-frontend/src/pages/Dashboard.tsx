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

  resolutionNote?: string;
}

function Dashboard() {
  const [complaints, setComplaints] = useState<Complaint[]>([]);
  const [notes, setNotes] = useState<{ [key: number]: string }>({});

  const cellStyle = {
    border: "1px solid #ccc",
    padding: "8px",
    whiteSpace: "nowrap" as const,
  };

  useEffect(() => {
    fetch("http://localhost:5196/api/complaints")
      .then((res) => res.json())
      .then((data) => setComplaints(data));
  }, []);

  // ================= ESCALATE =================
  const handleEscalate = async (id: number) => {
    const res = await fetch(
      `http://localhost:5196/api/complaints/escalate/${id}`,
      { method: "PUT" }
    );

    if (!res.ok) {
      alert(await res.text());
      return;
    }

    const updated = await res.json();

    setComplaints((prev) =>
      prev.map((c) => (c.id === id ? updated : c))
    );
  };

  // ================= RESOLVE =================
  const handleResolve = async (id: number) => {
    const note = notes[id] || "";

    const res = await fetch(
      `http://localhost:5196/api/complaints/resolve/${id}`,
      {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(note),
      }
    );

    if (!res.ok) {
      alert("Resolve failed");
      return;
    }

    const updated = await res.json();

    setComplaints((prev) =>
      prev.map((c) => (c.id === id ? updated : c))
    );
  };

  return (
    <div style={{ padding: "20px" }}>
      <h2>📊 Complaints Dashboard</h2>

      <div style={{ overflowX: "auto" }}>
        <table
          style={{
            width: "100%",
            borderCollapse: "collapse",
            minWidth: "1600px",
          }}
        >
          <thead>
            <tr>
              <th style={cellStyle}>Ticket</th>
              <th style={cellStyle}>User</th>
              <th style={cellStyle}>Phone</th>
              <th style={cellStyle}>Ward</th>
              <th style={cellStyle}>Area</th>
              <th style={cellStyle}>Address</th>
              <th style={cellStyle}>Category</th>
              <th style={cellStyle}>Department</th>
              <th style={cellStyle}>Status</th>
              <th style={cellStyle}>Escalation</th>
              <th style={cellStyle}>Assigned</th>
              <th style={cellStyle}>Created</th>
              <th style={cellStyle}>Due</th>
              <th style={cellStyle}>Resolved</th>
              <th style={cellStyle}>Resolution Note</th>
              <th style={cellStyle}>Action</th>
            </tr>
          </thead>

          <tbody>
            {complaints.map((c) => (
              <tr key={c.id}>
                <td style={cellStyle}>{c.ticketId}</td>
                <td style={cellStyle}>{c.userName}</td>
                <td style={cellStyle}>{c.callerPhone}</td>
                <td style={cellStyle}>{c.wardId}</td>
                <td style={cellStyle}>{c.areaId}</td>
                <td style={cellStyle}>{c.address}</td>
                <td style={cellStyle}>{c.category}</td>
                <td style={cellStyle}>{c.department}</td>
                <td style={cellStyle}>{c.status}</td>
                <td style={cellStyle}>{c.escalationLevel}</td>
                <td style={cellStyle}>{c.assignedTo}</td>
                <td style={cellStyle}>{new Date(c.createdAt).toLocaleString()}</td>
                <td style={cellStyle}>{new Date(c.stageDueAt).toLocaleString()}</td>
                <td style={cellStyle}>
                  {c.resolvedAt ? new Date(c.resolvedAt).toLocaleString() : "-"}
                </td>
                <td style={cellStyle}>
                  {c.resolutionNote || "-"}
                </td>

                {/* ACTION */}
                <td style={cellStyle}>
                  <div style={{ display: "flex", flexDirection: "column", gap: "5px" }}>

                    {/* NOTE INPUT */}
                    {c.status !== "Resolved" && (
                      <input
                        placeholder="Resolution note"
                        value={notes[c.id] || ""}
                        onChange={(e) =>
                          setNotes({
                            ...notes,
                            [c.id]: e.target.value,
                          })
                        }
                        style={{
                          padding: "5px",
                          fontSize: "12px",
                        }}
                      />
                    )}

                    <div style={{ display: "flex", gap: "5px" }}>
                      {/* ESCALATE */}
                      {c.status !== "Resolved" && c.escalationLevel < 3 && (
                        <button
                          onClick={() => handleEscalate(c.id)}
                          style={{
                            background: "orange",
                            color: "white",
                            border: "none",
                            padding: "5px",
                          }}
                        >
                          Escalate
                        </button>
                      )}

                      {/* RESOLVE */}
                      {c.status !== "Resolved" && (
                        <button
                          onClick={() => handleResolve(c.id)}
                          style={{
                            background: "green",
                            color: "white",
                            border: "none",
                            padding: "5px",
                          }}
                        >
                          Resolve
                        </button>
                      )}
                    </div>
                  </div>
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