import { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';
import { BrowserRouter, Routes, Route, Link } from "react-router-dom";

// Stílusfájlok (ha vannak)
import './index.css';

// Komponensek importálása
import App from './App.jsx';
import KitchenDashboard from "./KitchenDashboard.jsx";
import Order from "./Order.jsx";


const Navbar = () => (
  <nav className="main-nav" style={{
    display: 'flex',
    gap: '20px',
    padding: '15px 30px',
    background: '#2c3e50',
    color: 'white',
    alignItems: 'center',
    boxShadow: '0 2px 10px rgba(0,0,0,0.2)',
    position: 'fixed',
    top: 0,
    left: 0,
    right: 0,
    zIndex: 2000
  }}>
    <Link to="/" style={{ ...navLinkStyle, fontSize: '1.5rem', marginRight: '20px', padding: 0 }}>
      🍕 PizzaProjekt
    </Link>
    <Link to="/" style={navLinkStyle}>RendelésFelvétel</Link>
    <Link to="/kitchen" style={navLinkStyle}>Konyha</Link>
    <Link to="/delivery" style={navLinkStyle}>Átadás</Link>
  </nav>
);

const navLinkStyle = {
  color: '#ecf0f1',
  textDecoration: 'none',
  fontWeight: '600',
  padding: '8px 15px',
  borderRadius: '5px',
  transition: 'background 0.3s'
};

createRoot(document.getElementById('root')).render(
  <StrictMode>
    <BrowserRouter>
      <Navbar /> 
      <div style={{ paddingTop: '76px' }}>
        <Routes>
          <Route path="/" element={<App />} />
          <Route path="/kitchen" element={<KitchenDashboard />} />
          <Route path="/delivery" element={<Order />} />
        </Routes>
      </div>
    </BrowserRouter>
  </StrictMode>
);

