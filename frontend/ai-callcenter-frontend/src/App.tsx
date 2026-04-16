import Dashboard from "./pages/Dashboard";
import Login from "./pages/Login";

function App() {
  const role = localStorage.getItem("role");

  return (
    <div>
      {role ? <Dashboard /> : <Login />}
    </div>
  );
}

export default App;