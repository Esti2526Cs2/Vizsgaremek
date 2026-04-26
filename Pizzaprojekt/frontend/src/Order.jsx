import { useState, useEffect } from "react";
import "./Order.css";

function Order() {
  const [readyOrders, setReadyOrders] = useState([]);
  const [receivedOrders, setReceivedOrders] = useState([]);
  const [error, setError] = useState("");
  const today = new Date().toISOString().split("T")[0];
  const [selectedDate, setSelectedDate] = useState(today);

  const getPaymentMode = (order) => order.fizetesiMod || order.fizetes || "Nincs megadva";

  const isPickupOrder = (order) => {
    const addressText = (order.cim || "").toLowerCase();
    return addressText.includes("személyes átvétel") || addressText.includes("szemelyes atvetel");
  };

  const escapeHtml = (text) => String(text ?? "")
    .replace(/&/g, "&amp;")
    .replace(/</g, "&lt;")
    .replace(/>/g, "&gt;")
    .replace(/\"/g, "&quot;")
    .replace(/'/g, "&#039;");

  const getOrderItemsHtml = (order) => {
    const items = Array.isArray(order?.tetelek) ? order.tetelek : [];
    if (items.length === 0) {
      return "<li>Nincs tétel megadva.</li>";
    }

    return items
      .map((item) => {
        const toppings = Array.isArray(item?.feltetek) && item.feltetek.length > 0
          ? ` (+ ${item.feltetek.map((f) => escapeHtml(f.nev)).join(", ")})`
          : "";

        return `<li>${item.mennyiseg || 0}x ${escapeHtml(item.pizzaNev || "Ismeretlen pizza")}${toppings}</li>`;
      })
      .join("");
  };

  const printOrder = (order) => {
    const printWindow = window.open("", "_blank", "width=900,height=700");

    if (!printWindow) {
      setError("A nyomtatási ablakot a böngésző letiltotta.");
      return;
    }

    const html = `
      <!doctype html>
      <html lang="hu">
        <head>
          <meta charset="UTF-8" />
          <title>Rendelés #${escapeHtml(order.id)}</title>
          <style>
            body { font-family: Arial, sans-serif; padding: 24px; color: #111; }
            h1 { margin: 0 0 16px; font-size: 28px; }
            p { margin: 6px 0; font-size: 16px; }
            .section { margin-top: 18px; }
            ul { margin: 8px 0 0 20px; }
            li { margin: 6px 0; }
            .label { font-weight: 700; }
            @media print {
              body { padding: 0; }
            }
          </style>
        </head>
        <body>
          <h1>Rendelés #${escapeHtml(order.id)}</h1>
          <p><span class="label">Státusz:</span> ${escapeHtml(order.statusz || "ismeretlen")}</p>
          <p><span class="label">Rendelő:</span> ${escapeHtml(order.rendeloNev || "Ismeretlen rendelő")}</p>
          <p><span class="label">Telefon:</span> ${escapeHtml(order.telefonszam || "Nincs telefonszám")}</p>
          <p><span class="label">Idő:</span> ${escapeHtml(order.datum || "-")}</p>
          <p><span class="label">Cím:</span> ${escapeHtml(order.cim || "Nincs megadva")}</p>
          <p><span class="label">Fizetési mód:</span> ${escapeHtml(getPaymentMode(order))}</p>
          <p><span class="label">Végösszeg:</span> ${escapeHtml(order.osszeg ?? 0)} Ft</p>

          <div class="section">
            <p class="label">Tételek:</p>
            <ul>${getOrderItemsHtml(order)}</ul>
          </div>
        </body>
      </html>
    `;

    printWindow.document.open();
    printWindow.document.write(html);
    printWindow.document.close();
    printWindow.focus();
    printWindow.print();
    printWindow.close();
  };

  const load = async () => {
    try {
      setError("");
      const res = await fetch("https://localhost:7147/api/rendelesek");
      if (!res.ok) {
        throw new Error(`HTTP ${res.status}`);
      }

      const data = await res.json();
      const orders = Array.isArray(data) ? data : [];
      const readyFiltered = orders.filter((o) => {
        const status = (o.statusz || "").toLowerCase();
        return status === "átvehető" && (o.datum || "").startsWith(today);
      });

      const receivedFiltered = orders.filter((o) => {
        const status = (o.statusz || "").toLowerCase();
        return (status === "átadva" || status === "átvett") && (o.datum || "").startsWith(selectedDate);
      });

      setReadyOrders(readyFiltered);
      setReceivedOrders(receivedFiltered);
    } catch (err) {
      console.error("Hiba az átadásra váró rendelések betöltésekor:", err);
      setReadyOrders([]);
      setReceivedOrders([]);
      setError("Nem sikerült betölteni a rendeléseket.");
    }
  };

  useEffect(() => {
    load();
    const interval = setInterval(load, 5000);
    return () => clearInterval(interval);
  }, [selectedDate]);

  const deliver = async (id) => {
    try {
      const res = await fetch(`https://localhost:7147/api/rendelesek/${id}/status`, {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ statusz: "átadva" })
      });

      if (!res.ok) {
        throw new Error(`HTTP ${res.status}`);
      }

      load();
    } catch (err) {
      console.error("Hiba az átadás státusz frissítésekor:", err);
      setError("Nem sikerült frissíteni a rendelés státuszát.");
    }
  };

  return (
    <div className="order-delivery-container">
      <h1>📦 Átadó pult</h1>
      <h2>Mai átvehető rendelések</h2>

      {error && <p>{error}</p>}

      {readyOrders.length === 0 ? (
        <p className="empty-order-msg">Nincs átvehető rendelés.</p>
      ) : (
        <div className={`order-grid ${readyOrders.length === 1 ? "single-item" : ""}`}>
          {readyOrders.map((o) => (
            <div key={o.id} className="ready-order-card">
              <div className="ready-header">
                <h2>#{o.id}</h2>
                <span className="ready-status">{o.statusz || "ismeretlen"}</span>
              </div>

              <div className="ready-info">
                {isPickupOrder(o) && <p className="pickup-badge">Pultnál átvétel</p>}
                <p><strong>Rendelő:</strong> {o.rendeloNev || "Ismeretlen rendelő"}</p>
                <p><strong>Telefon:</strong> {o.telefonszam || "Nincs telefonszám"}</p>
                <p><strong>Idő:</strong> {o.datum || "-"}</p>
                <p><strong>Cím:</strong> {o.cim || "Nincs megadva"}</p>
                <p><strong>Fizetési mód:</strong> {getPaymentMode(o)}</p>
                <p><strong>Végösszeg:</strong> {o.osszeg ?? 0} Ft</p>
              </div>

              <div className="ready-items">
                {(o.tetelek || []).length === 0 ? (
                  <div className="ready-item-row">Nincs tétel megadva.</div>
                ) : (
                  (o.tetelek || []).map((item, index) => (
                    <div key={index} className="ready-item-row">
                      {item.mennyiseg || 0}x {item.pizzaNev || "Ismeretlen pizza"} {item.feltetek && item.feltetek.length > 0 && (
                        <div className="item-toppings">
                          + {item.feltetek.map((f) => f.nev).join(", ")}
                        </div>
                      )}
                    </div>
                  ))
                )}
              </div>

              <div className="order-action-row">
                <button className="print-btn" onClick={() => printOrder(o)}>
                  Nyomtatás
                </button>
                <button className="deliver-btn" onClick={() => deliver(o.id)}>
                  Átadás lezárása
                </button>
              </div>
            </div>
          ))}
        </div>
      )}

      <div className="order-filter-panel">
        <h2>Átvett rendelések szűrése dátum szerint</h2>
        <div className="order-filter-row">
          <label htmlFor="received-date">Dátum:</label>
          <input
            id="received-date"
            type="date"
            value={selectedDate}
            onChange={(e) => setSelectedDate(e.target.value)}
          />
        </div>
      </div>

      {receivedOrders.length === 0 ? (
        <p className="empty-order-msg">Nincs átvett rendelés a kiválasztott napon.</p>
      ) : (
        <div className={`order-grid ${receivedOrders.length === 1 ? "single-item" : ""}`}>
          {receivedOrders.map((o) => (
            <div key={`received-${o.id}`} className="ready-order-card received-order-card">
              <div className="ready-header">
                <h2>#{o.id}</h2>
                <span className="ready-status">{o.statusz || "ismeretlen"}</span>
              </div>

              <div className="ready-info">
                {isPickupOrder(o) && <p className="pickup-badge">Pultnál átvétel</p>}
                <p><strong>Rendelő:</strong> {o.rendeloNev || "Ismeretlen rendelő"}</p>
                <p><strong>Telefon:</strong> {o.telefonszam || "Nincs telefonszám"}</p>
                <p><strong>Idő:</strong> {o.datum || "-"}</p>
                <p><strong>Cím:</strong> {o.cim || "Nincs megadva"}</p>
                <p><strong>Fizetési mód:</strong> {getPaymentMode(o)}</p>
                <p><strong>Végösszeg:</strong> {o.osszeg ?? 0} Ft</p>
              </div>
              <div className="ready-items">
                {(o.tetelek || []).length === 0 ? (
                  <div className="ready-item-row">Nincs tétel megadva.</div>
                ) : (
                  (o.tetelek || []).map((item, index) => (
                    <div key={index} className="ready-item-row">
                      {item.mennyiseg || 0}x {item.pizzaNev || "Ismeretlen pizza"} {item.feltetek && item.feltetek.length > 0 && (
                        <div className="item-toppings">
                          + {item.feltetek.map((f) => f.nev).join(", ")}
                        </div>
                      )}
                    </div>
                  ))
                )}
              </div>

              <div className="order-action-row">
                <button className="print-btn" onClick={() => printOrder(o)}>
                  Nyomtatás
                </button>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}

export default Order;