import { useState, useEffect } from "react";
import "./App.css";

function App() {
  const PICKUP_OPTION_VALUE = "pickup";

  const [pizzak, setPizzak] = useState([]);
  const [toppings, setToppings] = useState([]);
  const [selectedPizza, setSelectedPizza] = useState(null);
  const [showPopup, setShowPopup] = useState(false);
  const [order, setOrder] = useState([]);
  const [time, setTime] = useState("--:--:--");

  const [orderPayment, setOrderPayment] = useState("keszpenz");
  const [customerName, setCustomerName] = useState("");
  const [customerPhone, setCustomerPhone] = useState("");
  const [selectedCustomerId, setSelectedCustomerId] = useState(null);
  const [savedAddresses, setSavedAddresses] = useState([]);
  const [selectedAddressId, setSelectedAddressId] = useState(null);
  const [selectedAddressOption, setSelectedAddressOption] = useState("");
  const [isPickupAtCounter, setIsPickupAtCounter] = useState(false);

  const [city, setCity] = useState("");
  const [zip, setZip] = useState("9700");
  const [street, setStreet] = useState("");
  const [houseNumber, setHouseNumber] = useState("");
  const [floorDoor, setFloorDoor] = useState("");

  const [orderStatus, setOrderStatus] = useState("nincs rendelés");
  const [selectedToppingIds, setSelectedToppingIds] = useState([]);

  // ÚJ: Állapotok a keresőhöz (Autocomplete)
  const [nameSuggestions, setNameSuggestions] = useState([]);
  const [showSuggestions, setShowSuggestions] = useState(false);

  useEffect(() => {
    const interval = setInterval(() => {
      setTime(new Date().toLocaleString("hu-HU"));
    }, 1000);
    return () => clearInterval(interval);
  }, []);

  useEffect(() => {
    fetch("https://localhost:7147/api/Pizza")
      .then((r) => r.json())
      .then((data) => setPizzak(data))
      .catch(err => console.error("Nem sikerült lekérni a pizzákat", err));
  }, []);

  useEffect(() => {
    fetch("https://localhost:7147/api/Feltetek")
      .then((r) => r.json())
      .then((data) => setToppings(data))
      .catch(err => console.error("Nem sikerült lekérni a feltéteket", err));
  }, []);

  // --- AUTOCOMPLETE LOGIKA (Vevő keresése) ---
  async function handleNameChange(e) {
    const input = e.target.value;
    setCustomerName(input);
    setSelectedCustomerId(null);
    setSavedAddresses([]);
    setSelectedAddressId(null);
    setSelectedAddressOption("");
    setIsPickupAtCounter(false);

    if (input.trim().length >= 2) {
      try {
        // Lekérjük az összes vásárlót (vagy a kereső végpontot, ha van)
        const res = await fetch("https://localhost:7147/api/Vasarlo");
        if (res.ok) {
          const data = await res.json();
          // Szűrjük a listát arra, amit a pincér épp gépel (kis/nagybetű nem számít)
          const filtered = data.filter(v => v.nev && v.nev.toLowerCase().includes(input.toLowerCase()));
          setNameSuggestions(filtered);
          setShowSuggestions(true);
        }
      } catch (err) {
        console.error("Hiba a vásárlók lekérésekor:", err);
      }
    } else {
      setShowSuggestions(false);
    }
  }

  function fillAddressFields(address) {
    setCity(address?.varos || "");
    setZip(address?.iranyitoszam || "");
    setStreet(address?.utca || "");
    setHouseNumber(address?.hazszam || "");
    setFloorDoor(address?.emeletAjto || "");
  }

  function handleSavedAddressChange(e) {
    const value = e.target.value;
    setSelectedAddressOption(value);

    if (value === PICKUP_OPTION_VALUE) {
      setSelectedAddressId(null);
      setIsPickupAtCounter(true);
      setCity("");
      setZip("");
      setStreet("");
      setHouseNumber("");
      setFloorDoor("");
      return;
    }

    if (!value) {
      setSelectedAddressId(null);
      setIsPickupAtCounter(false);
      setCity("Szombathely");
      setZip("9700");
      setStreet("");
      setHouseNumber("");
      setFloorDoor("");
      return;
    }

    const selectedAddress = savedAddresses.find((address) => String(address.cimId) === value);
    if (selectedAddress) {
      setIsPickupAtCounter(false);
      setSelectedAddressId(selectedAddress.cimId ?? null);
      fillAddressFields(selectedAddress);
    }
  }

  function selectCustomer(v) {
    setSelectedCustomerId(v.rendeloAzonosito || null);
    setCustomerName(v.nev || "");
    setCustomerPhone(v.telefonszam || "");

    const addresses = Array.isArray(v.cimek) && v.cimek.length > 0
      ? v.cimek
      : (v.cim ? [v.cim] : []);

    setSavedAddresses(addresses);

    if (addresses.length > 0) {
      setSelectedAddressId(addresses[0].cimId ?? null);
      setSelectedAddressOption(String(addresses[0].cimId ?? ""));
      setIsPickupAtCounter(false);
      fillAddressFields(addresses[0]);
    } else {
      setSelectedAddressId(null);
      setSelectedAddressOption("");
      setIsPickupAtCounter(false);
      fillAddressFields(null);
    }

    setShowSuggestions(false);
  }

  // --- POPUP ÉS RENDELÉS LOGIKA ---
  function openToppingScreen(pizza) {
    setSelectedPizza(pizza);
    setSelectedToppingIds([]);
    setShowPopup(true);
    document.body.classList.add("dimmed");
  }

  function closeToppingScreen() {
    setShowPopup(false);
    document.body.classList.remove("dimmed");
  }

  function toggleTopping(id) {
    setSelectedToppingIds((prev) =>
      prev.includes(id) ? prev.filter((x) => x !== id) : [...prev, id]
    );
  }

  function saveToppings() {
    const selected = toppings.filter((t) => selectedToppingIds.includes(t.id));
    addToOrder(selectedPizza, selected);
    closeToppingScreen();
  }

  // Csak pizza gomb
  function saveOnlyPizza() {
    addToOrder(selectedPizza, []);
    closeToppingScreen();
  }

  function addToOrder(pizza, toppingList) {
    setOrder((prev) => [
      ...prev,
      {
        pizza: pizza,
        toppings: toppingList,
        qty: 1,
      },
    ]);
  }

  // --- KALKULÁCIÓK ---
  function changeQty(index, delta) {
    setOrder((prev) => {
      const updated = prev.map((item, i) =>
        i === index ? { ...item, qty: item.qty + delta } : item
      );
      return updated.filter((item) => item.qty > 0);
    });
  }

  function calcItemTotal(item) {
    const base = item.pizza.ar;
    const toppingSum = item.toppings.reduce((sum, t) => sum + t.ar, 0);
    return (base + toppingSum) * item.qty;
  }

  function calcOrderTotal() {
    return order.reduce((sum, item) => sum + calcItemTotal(item), 0);
  }

  // --- KÜLDÉS ---
  async function sendOrder() {
    try {
      const pickupLabel = "Személyes átvétel (pultnál)";
      const effectiveCustomerName = isPickupAtCounter ? "Pult" : customerName;
      const effectiveCustomerPhone = isPickupAtCounter ? "" : customerPhone;
      const effectiveCustomerId = isPickupAtCounter ? null : selectedCustomerId;

      if (order.length === 0) {
        alert("Nincs tétel a rendelésben!");
        return;
      }
      if (!isPickupAtCounter && (customerName.trim() === "" || customerPhone.trim() === "")) {
        alert("Kérlek add meg a neved és telefonszámod!");
        return;
      }
      if (!isPickupAtCounter && (city.trim() === "" || zip.trim() === "" || street.trim() === "" || houseNumber.trim() === "")) {
        alert("Kérlek töltsd ki a cím mezőket!");
        return;
      }

      const tetelMap = order.reduce((acc, item) => {
        const pizzaId = item.pizza.id;
        if (!acc[pizzaId]) {
          acc[pizzaId] = { pizzaId, mennyiseg: 0 };
        }
        acc[pizzaId].mennyiseg += item.qty;
        return acc;
      }, {});

      const saveComplexDto = {
        vasarlo: {
          rendeloAzonosito: effectiveCustomerId,
          nev: effectiveCustomerName,
          telefonszam: effectiveCustomerPhone,
        },
        kiszallitasiHely: {
          cimId: selectedAddressId,
          varos: isPickupAtCounter ? pickupLabel : city,
          iranyitoszam: isPickupAtCounter ? "" : zip,
          utca: isPickupAtCounter ? "" : street,
          hazszam: isPickupAtCounter ? "" : houseNumber,
          emeletAjto: isPickupAtCounter ? "" : floorDoor,
        },
        rendeles: {
          datum: new Date().toISOString(),
          statusz: "folyamatban",
          fizetesiMod: orderPayment,
        },
        tetelLista: Object.values(tetelMap),
      };

      const orderResponse = await fetch("https://localhost:7147/api/Tartalmaz/savecomplex", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(saveComplexDto),
      });

      if (!orderResponse.ok) {
        const details = await orderResponse.text();
        throw new Error(`Nem sikerült menteni a rendelést. (${orderResponse.status}) ${details}`);
      }

      const orderData = await orderResponse.json();
      const orderId = orderData.RendelesId ?? orderData.rendelesId;
      if (!orderId) throw new Error("Nem sikerült beolvasni a rendelés azonosítóját.");

      alert("Rendelés elküldve!");
      setOrderStatus("sütés alatt");
      setTimeout(() => setOrderStatus("úton"), 5000);
      setTimeout(() => setOrderStatus("kiszállítva"), 10000);

      setOrder([]);
      setSelectedCustomerId(null);
      setSavedAddresses([]);
      setSelectedAddressId(null);
      setSelectedAddressOption("");
      setIsPickupAtCounter(false);
      setCustomerName("");
      setCustomerPhone("");
      setCity("");
      setZip("9700");
      setStreet("");
      setHouseNumber("");
      setFloorDoor("");
    } catch (err) {
      alert(err.message);
    }
  }

  return (
    <>
      <div className="app-shell">
        <main className="main-layout">

          {/* PIZZALISTA PANEL */}
          <section className="menu-panel">
            <h2 className="menu-title">Pizzák</h2>
            <div className="pizza-grid">
              {pizzak.map((p) => (
                <article key={p.id} className="pizza-card" onClick={() => openToppingScreen(p)}>
                  <img src={p.image || "kepek/default.png"} alt={p.nev} className="pizza-img" />
                  <div className="pizza-info">
                    <h3 className="pizza-name">{p.id}. {p.nev}</h3>
                    <p className="pizza-price">{p.ar} Ft</p>
                  </div>
                </article>
              ))}
            </div>
          </section>

          {/* RENDELÉSI PANEL */}
          <section className="order-panel">
            <header className="topbar">
              <div className="topbar-right"><span className="top-right-time">{time}</span></div>
            </header>

            <div className="order-scroll-area">
              <header className="order-header">
                <h2 className="order-title">Rendelés</h2>
                <p className="order-subtitle">Rendelés összesítése</p>
              </header>

              <div className="order-status"><strong>Státusz:</strong> {orderStatus}</div>

              {order.length > 0 && (
                <div className="customer-details">

                {/* AUTOCOMPLETE NÉV MEZŐ */}
                <div style={{ gridColumn: 'span 2', position: 'relative', width: '100%' }}>
                  <input
                    type="text"
                    value={customerName}
                    onChange={handleNameChange}
                    onFocus={() => { if (nameSuggestions.length > 0) setShowSuggestions(true); }}
                    placeholder="Név"
                    autoComplete="off"
                  />
                  {showSuggestions && nameSuggestions.length > 0 && (
                    <ul style={{
                      position: "absolute", top: "100%", left: 0, width: "100%",
                      background: "#1e293b", border: "1px solid #e4e00e", borderRadius: "0 0 12px 12px",
                      listStyle: "none", padding: 0, margin: 0, zIndex: 100,
                      maxHeight: "200px", overflowY: "auto", boxShadow: "0 10px 15px rgba(0,0,0,0.5)"
                    }}>
                      {nameSuggestions.map((v) => (
                        <li
                          key={v.rendeloAzonosito}
                          onClick={() => selectCustomer(v)}
                          style={{
                            padding: "10px 15px", cursor: "pointer", borderBottom: "1px solid #334155", color: "#e5e7eb"
                          }}
                          onMouseEnter={(e) => { e.target.style.background = "#334155"; e.target.style.color = "#e4e00e"; }}
                          onMouseLeave={(e) => { e.target.style.background = "transparent"; e.target.style.color = "#e5e7eb"; }}
                        >
                          <strong>{v.nev}</strong> <span style={{ fontSize: '0.8rem', color: '#9ca3af', marginLeft: '5px' }}>{v.telefonszam}</span> <span style={{ fontSize: '0.8rem', color: '#9ca3af', marginLeft: '5px' }}>{Array.isArray(v.cimek) && v.cimek.length > 0 ? v.cimek[0].utca : v.cim ? v.cim.utca : ""}</span> <span style={{ fontSize: '0.8rem', color: '#9ca3af', marginLeft: '5px' }}>{Array.isArray(v.cimek) && v.cimek.length > 0 ? v.cimek[0].hazszam : v.cim ? v.cim.hazszam : ""}</span>
                        </li>
                      ))}
                    </ul>
                  )}
                </div>

                <div style={{ gridColumn: 'span 2' }}>
                  <select value={selectedAddressOption} onChange={handleSavedAddressChange}>
                    <option value="">Új cím megadása kézzel</option>
                    <option value={PICKUP_OPTION_VALUE}>Személyes átvétel (pultnál)</option>
                    {savedAddresses.map((address, index) => (
                      <option key={address.cimId ?? index} value={address.cimId ?? ""}>
                        {`${address.varos || ""}, ${address.utca || ""} ${address.hazszam || ""}${address.emeletAjto ? `, ${address.emeletAjto}` : ""}`}
                      </option>
                    ))}
                  </select>
                </div>

                <input type="text" value={city} onChange={(e) => { setSelectedAddressId(null); setCity(e.target.value); }} placeholder="Város" />
                <input type="text" value={street} onChange={(e) => { setSelectedAddressId(null); setStreet(e.target.value); }} placeholder="Utca" />

                <input type="text" value={houseNumber} onChange={(e) => { setSelectedAddressId(null); setHouseNumber(e.target.value); }} placeholder="Házszám" />
                <input type="text" value={floorDoor} onChange={(e) => { setSelectedAddressId(null); setFloorDoor(e.target.value); }} placeholder="Emelet/Ajtó" />

                <input type="text" value={customerPhone} onChange={(e) => setCustomerPhone(e.target.value)} placeholder="Telefon" />
                <input type="text" value={zip} onChange={(e) => { setSelectedAddressId(null); setZip(e.target.value); }} placeholder="Irányítószám" />

                <select value={orderPayment} onChange={(e) => setOrderPayment(e.target.value)}>
                  <option value="keszpenz">Készpénz</option>
                  <option value="kartya">Bankkártya</option>
                </select>
                </div>
              )}

              <div className="order-items">
                {order.map((item, index) => (
                  <div className="order-item" key={index}>
                    <div className="item-name">{item.pizza.nev}</div>
                    <div className="item-toppings">
                      {item.toppings.map((t) => `${t.nev}`).join(", ")}
                    </div>
                    <div className="item-qty-controls">
                      <button className="qty-btn" onClick={() => changeQty(index, -1)}>−</button>
                      <span className="qty-value">{item.qty}</span>
                      <button className="qty-btn" onClick={() => changeQty(index, 1)}>+</button>
                    </div>
                    <div className="item-total">{calcItemTotal(item)} Ft</div>
                  </div>
                ))}
              </div>
            </div>

            <footer className="order-footer">
              <div className="order-summary"><span>Összesítés:</span> <strong>{calcOrderTotal()} Ft</strong></div>
              <div className="order-actions">
                <button className="btn ghost" onClick={() => setOrder([])}>törlés</button>
                <button className="btn primary" onClick={sendOrder}>küldés: konyha</button>
              </div>
            </footer>
          </section>
        </main>
      </div>

      {/* POPUP A FELTÉTEKHEZ */}
      {showPopup && (
        <div id="topping-screen" className="topping-screen active">
          <div className="topping-box">
            <h2>{selectedPizza?.nev}</h2>
            <div className="topping-content">
              {toppings.map((t) => (
                <label key={t.id}>
                  <input type="checkbox" checked={selectedToppingIds.includes(t.id)} onChange={() => toggleTopping(t.id)} />
                  {t.nev} (+{t.ar} Ft)
                </label>
              ))}
            </div>
            <div className="topping-buttons">
              <button className="btn ghost" onClick={closeToppingScreen}>Mégse</button>
              <button className="btn primary" onClick={saveToppings}>Hozzáadás</button>
              {/* JAVÍTOTT GOMB: Csak a pizzát menti */}
              <button className="btn primary2" onClick={saveOnlyPizza}>CSAK PIZZA!!</button>
            </div>
          </div>
        </div>
      )}
    </>
  );
}

export default App;