const BASE_URL = "http://localhost:5196/api/complaints";

export const getComplaints = async () => {
  const response = await fetch(BASE_URL);
  console.log("Status:", response.status);

  const data = await response.json();
  console.log("Data from API:", data);

  return data;
};