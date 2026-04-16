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
  const [filteredData, setFilteredData] = useState<Complaint[]>([]);

  const [notes, setNotes] = useState<{ [key: number]: string }>({});
  const [images, setImages] = useState<{ [key: number]: string }>({});

  const [selectedDept, setSelectedDept] = useState("");
  const [selectedStatus, setSelectedStatus] = useState("");
  const [searchTicket, setSearchTicket] = useState("");
  const [sortOrder, setSortOrder] = useState("new");

  const role = localStorage.getItem("role");
  const department = localStorage.getItem("department");
  const currentUser = localStorage.getItem("username");

  const cellStyle = {
    border: "1px solid #ccc",
    padding: "8px",
    textAlign: "center" as const,
    whiteSpace: "nowrap" as const,
  };

  const departmentList = [
    "Bridge Cell","Drainage Projects","Town Development (B.P.)","Street Light",
    "JnNURM (BSUP)","Sewage Disposal Works","Building Project","Road",
    "Storm Water Drainage","Water Works","Land & Estate (T.P.)",
    "Town Development Department (T.P.)","IT","Accounts","Audit","Census",
    "P R O","Election","Land & Estate (Acquisition)","Shops & Establishment",
    "Assessment Department","Solid Waste Management","Health Department",
    "I.C.D.S. Department","Birth, Death & Marriage"
  ];

  // FETCH
  useEffect(() => {
    fetch("http://localhost:5196/api/complaints")
      .then((res) => res.json())
      .then((data) => {
        if (role === "Technician") {
          data = data.filter(
            (c: any) =>
              c.department?.toLowerCase().trim() ===
              department?.toLowerCase().trim()
          );
        }
        setComplaints(data);
        setFilteredData(data);
      });
  }, []);

  // FILTER
  useEffect(() => {
    let data = [...complaints];

    if (selectedDept) {
      data = data.filter((c) => c.department === selectedDept);
    }

    if (selectedStatus) {
      if (selectedStatus === "Escalated") {
        data = data.filter((c) => c.escalationLevel > 0 && c.status !== "Resolved");
      } else if (selectedStatus === "Pending") {
        data = data.filter((c) => c.status !== "Resolved");
      } else {
        data = data.filter((c) => c.status === selectedStatus);
      }
    }

    if (searchTicket) {
      data = data.filter((c) =>
        c.ticketId.toLowerCase().includes(searchTicket.toLowerCase())
      );
    }

    data.sort((a, b) =>
      sortOrder === "new"
        ? new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime()
        : new Date(a.createdAt).getTime() - new Date(b.createdAt).getTime()
    );

    setFilteredData(data);
  }, [selectedDept, selectedStatus, searchTicket, sortOrder, complaints]);

  // BASE64
  const toBase64 = (file: File, id: number) => {
    const reader = new FileReader();
    reader.onload = () => {
      setImages((prev) => ({ ...prev, [id]: reader.result as string }));
    };
    reader.readAsDataURL(file);
  };

  const canResolve = (id: number) =>
    (notes[id] ?? "").trim() && (images[id] ?? "").trim();

  // ACTIONS
  const handleResolve = async (id: number) => {
    const res = await fetch(`http://localhost:5196/api/complaints/resolve/${id}`, {
      method: "PUT",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({
        note: notes[id],
        imageBase64: images[id],
        resolvedBy: currentUser,
      }),
    });

    const updated = await res.json();
    setComplaints((prev) => prev.map((c) => (c.id === id ? updated : c)));
  };

  const handleEscalate = async (id: number) => {
    const res = await fetch(`http://localhost:5196/api/complaints/escalate/${id}`, {
      method: "PUT",
    });

    if (!res.ok) {
      alert(await res.text());
      return;
    }

    const updated = await res.json();
    setComplaints((prev) => prev.map((c) => (c.id === id ? updated : c)));
  };

  const handleLogout = () => {
    localStorage.clear();
    window.location.href = "/";
  };

  return (
    <div style={{ padding: "20px" }}>
      
      {/* HEADER */}
      <div style={{ display: "flex", justifyContent: "space-between" }}>
        <h2>Complaints Dashboard</h2>
        <button onClick={handleLogout} style={{ background: "red", color: "white", padding: "6px 10px", border: "none", borderRadius: "5px" }}>
          Logout
        </button>
      </div>

      {/* FILTER BAR */}
      <div style={{ display: "flex", gap: "10px", margin: "15px 0", flexWrap: "wrap" }}>
        <select value={selectedDept} onChange={(e) => setSelectedDept(e.target.value)}>
          <option value="">All Departments</option>
          {departmentList.map((d, i) => (
            <option key={i} value={d}>{d}</option>
          ))}
        </select>

        <select value={selectedStatus} onChange={(e) => setSelectedStatus(e.target.value)}>
          <option value="">All Status</option>
          <option value="New">New</option>
          <option value="Resolved">Resolved</option>
          <option value="Pending">Pending</option>
          <option value="Escalated">Escalated</option>
        </select>

        <input
          placeholder="Search Ticket ID"
          value={searchTicket}
          onChange={(e) => setSearchTicket(e.target.value)}
        />

        <select value={sortOrder} onChange={(e) => setSortOrder(e.target.value)}>
          <option value="new">Newest</option>
          <option value="old">Oldest</option>
        </select>
      </div>

      {/* TABLE */}
      <div style={{ overflowX: "auto" }}>
        <table style={{ width: "100%", borderCollapse: "collapse", minWidth: "1500px" }}>
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
              <th style={cellStyle}>Resolved By</th>
              <th style={cellStyle}>Proof</th>
              <th style={cellStyle}>Action</th>
            </tr>
          </thead>

          <tbody>
            {filteredData.map((c) => (
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
                <td style={cellStyle}>{c.resolvedBy || "-"}</td>

                <td style={cellStyle}>
                  {c.resolutionImageBase64 ? <img src={c.resolutionImageBase64} width="60" /> : "-"}
                </td>

                <td style={cellStyle}>
                  {c.status !== "Resolved" && (
                    <div style={{ display: "flex", flexDirection: "column", gap: "5px" }}>
                      <input placeholder="Note" onChange={(e) => setNotes({ ...notes, [c.id]: e.target.value })} />

                      <input type="file" onChange={(e) => e.target.files && toBase64(e.target.files[0], c.id)} />

                      <input type="file" accept="image/*" capture="environment"
                        onChange={(e) => e.target.files && toBase64(e.target.files[0], c.id)}
                      />

                      <div style={{ display: "flex", gap: "5px" }}>
                        {(c.escalationLevel ?? 0) < 3 && (
                          <button style={{ background: "#ff9800", color: "white", border: "none", padding: "5px", borderRadius: "5px" }}
                            onClick={() => handleEscalate(c.id)}>
                            Escalate
                          </button>
                        )}

                        <button
                          style={{
                            background: canResolve(c.id) ? "#4CAF50" : "#9e9e9e",
                            color: "white",
                            border: "none",
                            padding: "5px",
                            borderRadius: "5px",
                          }}
                          disabled={!canResolve(c.id)}
                          onClick={() => handleResolve(c.id)}
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