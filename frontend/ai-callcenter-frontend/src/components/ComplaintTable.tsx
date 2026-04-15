type Complaint = {
  id?: number;
  ticketId?: string;
  TicketId?: string;
  userName?: string;
  UserName?: string;
  category?: string;
  Category?: string;
  status?: string;
  Status?: string;
  assignedTo?: string;
  AssignedTo?: string;
  escalationLevel?: number;
  EscalationLevel?: number;
};

type Props = {
  complaints: Complaint[];
};

function ComplaintTable({ complaints }: Props) {
  return (
    <table border={1} cellPadding={10} style={{ borderCollapse: "collapse" }}>
      <thead>
        <tr>
          <th>Ticket ID</th>
          <th>User Name</th>
          <th>Category</th>
          <th>Status</th>
          <th>Assigned To</th>
          <th>Escalation Level</th>
        </tr>
      </thead>

      <tbody>
        {complaints.length === 0 ? (
          <tr>
            <td colSpan={6}>No complaints found</td>
          </tr>
        ) : (
          complaints.map((c, index) => (
            <tr key={index}>
              <td>{c.ticketId || c.TicketId}</td>
              <td>{c.userName || c.UserName}</td>
              <td>{c.category || c.Category}</td>
              <td>{c.status || c.Status}</td>
              <td>{c.assignedTo || c.AssignedTo}</td>
              <td>{c.escalationLevel ?? c.EscalationLevel}</td>
            </tr>
          ))
        )}
      </tbody>
    </table>
  );
}

export default ComplaintTable;