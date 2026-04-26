export  async function loadPizzas() {
 
const response = fetch("https://localhost:7147/api/pizza")
  const valasz =  await response.then(r => r.json())
  return valasz;}