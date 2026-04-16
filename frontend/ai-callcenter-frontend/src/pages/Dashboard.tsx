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

  escalationLevel: number;
  assignedTo: string;

  stageDueAt: string;

  resolvedAt?: string;
  resolutionNote?: string;
  resolutionImageBase64?: string;
  resolvedBy?: string;
}

function Dashboard() {
  const [complaints, setComplaints] = useState<Complaint[]>([]);
  const [notes, setNotes] = useState<{ [key: number]: string }>({});
  const [images, setImages] = useState<{ [key: number]: string }>({});

  const cellStyle = {
    border: "1px solid #ccc",
    padding: "8px",
    whiteSpace: "nowrap" as const,
    verticalAlign: "top",
  };

  const currentUser = "Admin Officer";

  useEffect(() => {
    fetch("http://localhost:5196/api/complaints")
      .then((res) => res.json())
      .then((data) => setComplaints(data));
  }, []);

  const toBase64 = (file: File, id: number) => {
    const reader = new FileReader();
    reader.onload = () => {
      setImages((prev) => ({
        ...prev,
        [id]: reader.result as string,
      }));
    };
    reader.readAsDataURL(file);
  };

  const canResolve = (id: number) => {
    return (
      (notes[id] ?? "").trim().length > 0 &&
      (images[id] ?? "").trim().length > 0
    );
  };

  const handleResolve = async (id: number) => {
    const res = await fetch(
      `http://localhost:5196/api/complaints/resolve/${id}`,
      {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          note: notes[id],
          imageBase64: images[id],
          resolvedBy: currentUser,
        }),
      }
    );

    const updated = await res.json();

    setComplaints((prev) =>
      prev.map((c) => (c.id === id ? updated : c))
    );
  };

  const handleEscalate = async (id: number) => {
    const res = await fetch(
      `http://localhost:5196/api/complaints/escalate/${id}`,
      { method: "PUT" }
    );

    const updated = await res.json();

    setComplaints((prev) =>
      prev.map((c) => (c.id === id ? updated : c))
    );
  };

  return (
    <div style={{ padding: "20px" }}>

      {/* ❌ REMOVED: AI Call Center Admin Panel text */}

      <h2 style={{ marginBottom: "15px" }}>
        📊 Complaints Dashboard
      </h2>

      <div style={{ overflowX: "auto" }}>
        <table
          style={{
            width: "100%",
            borderCollapse: "collapse",
            minWidth: "2300px",
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
              <th style={cellStyle}>Description</th>
              <th style={cellStyle}>Status</th>
              <th style={cellStyle}>Escalation</th>
              <th style={cellStyle}>Assigned</th>
              <th style={cellStyle}>Created</th>
              <th style={cellStyle}>Due</th>
              <th style={cellStyle}>Resolved</th>
              <th style={cellStyle}>Resolved By</th>
              <th style={cellStyle}>Note</th>
              <th style={cellStyle}>Image</th>
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
                <td style={cellStyle}>{c.description}</td>
                <td style={cellStyle}>{c.status}</td>
                <td style={cellStyle}>{c.escalationLevel}</td>
                <td style={cellStyle}>{c.assignedTo}</td>
                <td style={cellStyle}>
                  {new Date(c.createdAt).toLocaleString()}
                </td>
                <td style={cellStyle}>
                  {new Date(c.stageDueAt).toLocaleString()}
                </td>
                <td style={cellStyle}>
                  {c.resolvedAt
                    ? new Date(c.resolvedAt).toLocaleString()
                    : "-"}
                </td>
                <td style={cellStyle}>{c.resolvedBy || "-"}</td>
                <td style={cellStyle}>{c.resolutionNote || "-"}</td>

                <td style={cellStyle}>
                  {c.resolutionImageBase64 ? (
                    <img
                      src={c.resolutionImageBase64}
                      style={{
                        width: 80,
                        height: 80,
                        objectFit: "cover",
                        borderRadius: "6px",
                      }}
                    />
                  ) : (
                    "-"
                  )}
                </td>

                {/* ================= ACTION ================= */}
                <td style={cellStyle}>
                  {c.status !== "Resolved" && (
                    <div style={{ display: "flex", flexDirection: "column", gap: "5px" }}>

                      <input
                        placeholder="Resolution note"
                        value={notes[c.id] || ""}
                        onChange={(e) =>
                          setNotes({ ...notes, [c.id]: e.target.value })
                        }
                      />

                      <input
                        type="file"
                        accept="image/*"
                        onChange={(e) => {
                          if (e.target.files?.[0]) {
                            toBase64(e.target.files[0], c.id);
                          }
                        }}
                      />

                      <input
                        type="file"
                        accept="image/*"
                        capture="environment"
                        onChange={(e) => {
                          if (e.target.files?.[0]) {
                            toBase64(e.target.files[0], c.id);
                          }
                        }}
                      />

                      <div style={{ display: "flex", gap: "6px" }}>

                        {/* 🔥 ESCALATE BUTTON (RESTORED COLOR) */}
                        {c.status !== "Resolved" &&
                          (c.escalationLevel ?? 0) < 3 && (
                            <button
                              onClick={() => handleEscalate(c.id)}
                              style={{
                                background: "#ff9800",
                                color: "white",
                                border: "none",
                                padding: "6px 10px",
                                borderRadius: "5px",
                                cursor: "pointer",
                                fontWeight: "bold",
                              }}
                            >
                              Escalate
                            </button>
                          )}

                        {/* 🔥 RESOLVE BUTTON (RESTORED COLOR) */}
                        <button
                          onClick={() => handleResolve(c.id)}
                          disabled={!canResolve(c.id)}
                          style={{
                            background: canResolve(c.id) ? "#2e7d32" : "#9e9e9e",
                            color: "white",
                            border: "none",
                            padding: "6px 10px",
                            borderRadius: "5px",
                            cursor: canResolve(c.id) ? "pointer" : "not-allowed",
                            fontWeight: "bold",
                          }}
                        >
                          Resolve
                        </button>
                      </div>
                    </div>
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