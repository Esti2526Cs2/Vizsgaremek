import { useEffect, useRef, useState } from "react";
import "./kitchen.css";

function CountDownTimer({ initialSeconds, onComplete }) {
  const [seconds, setSeconds] = useState(initialSeconds);
  const hasCompleted = useRef(false);

  useEffect(() => {
    setSeconds(initialSeconds);
    hasCompleted.current = false;
  }, [initialSeconds]);

  useEffect(() => {
    if (seconds <= 0) {
      if (!hasCompleted.current) {
        hasCompleted.current = true;
        onComplete();
      }

      return;
    }

    const timer = setTimeout(() => {
      setSeconds((prev) => Math.max(prev - 1, 0));
    }, 1000);

    return () => clearTimeout(timer);
  }, [seconds, onComplete]);

  const formatTime = (totalSeconds) => {
    const min = Math.floor(totalSeconds / 60);
    const sec = totalSeconds % 60;
    return `${min}:${sec < 10 ? "0" : ""}${sec}`;
  };

  return (
    <div className="timer-wrapper">
      <div className="live-timer">⏳ Hátralévő idő: {formatTime(seconds)}</div>
      <div className="progress-bar-bg">
        <div className="progress-bar-fill"></div>
      </div>
    </div>
  );
}

function KitchenDashboard() {
  const [orders, setOrders] = useState([]);
  const [statusFilter, setStatusFilter] = useState(["összes"]);
  const [onlyToday, setOnlyToday] = useState(true);
  const today = new Date().toISOString().split("T")[0];

  useEffect(() => {
    loadOrders();
    const interval = setInterval(loadOrders, 5000);
    return () => clearInterval(interval);
  }, []);

  async function loadOrders() {
    try {
      const res = await fetch("https://localhost:7147/api/rendelesek");
      if (!res.ok) {
        throw new Error(`HTTP ${res.status}`);
      }

      const data = await res.json();
      setOrders(Array.isArray(data) ? data : []);
    } catch (err) {
      console.error("Hiba a rendelések betöltésekor:", err);
      setOrders([]);
    }
  }

  async function updateStatus(orderId, newStatus) {
    try {
      await fetch(`https://localhost:7147/api/rendelesek/${orderId}/status`, {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ statusz: newStatus })
      });
      loadOrders();
    } catch (err) {
      console.error("Hiba a státusz frissítésekor:", err);
    }
  }

  async function setBakeTimeApi(orderId, minutes) {
    try {
      await fetch(`https://localhost:7147/api/rendelesek/${orderId}/sutesiido`, {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ sutesiIdo: minutes })
      });
    } catch (err) {
      console.error("Hiba a sütési idő mentésekor:", err);
    }
  }

  async function startBakingProcess(orderId) {
    await setBakeTimeApi(orderId, 10);
    await updateStatus(orderId, "sütés alatt");
  }

  async function markAsReadyImmediately(orderId) {
    const confirmed = window.confirm(
      "Biztosan azonnal készre állítod ezt a rendelést? A visszaszámláló 0-ra vált."
    );

    if (!confirmed) {
      return;
    }

    setOrders((prev) =>
      prev.map((order) =>
        order.id === orderId ? { ...order, sutesiIdo: 0, statusz: "kész" } : order
      )
    );

    await setBakeTimeApi(orderId, 0);
    await updateStatus(orderId, "kész");
  }

  function toggleStatusFilter(value) {
    if (value === "mai") {
      setOnlyToday((prev) => !prev);
      return;
    }

    setStatusFilter((prev) => {
      if (value === "összes") {
        return ["összes"];
      }

      const withoutAll = prev.filter((item) => item !== "összes");

      if (withoutAll.includes(value)) {
        const next = withoutAll.filter((item) => item !== value);
        return next.length === 0 ? ["összes"] : next;
      }

      return [...withoutAll, value];
    });
  }

  const filteredOrders = orders.filter((order) => {
    const isTodayOrder = (order.datum || "").startsWith(today);
    if (onlyToday && !isTodayOrder) {
      return false;
    }

    if (statusFilter.includes("összes")) {
      return true;
    }

    return statusFilter.includes((order.statusz || "").toLowerCase());
  });

  const statusPriority = {
    "folyamatban": 1,
    "függő": 2,
    "sütés alatt": 3,
    "kész": 4,
    "átvehető": 5,
    "átadva": 6,
  };

  const sortedOrders = [...filteredOrders].sort((a, b) => {
    const statusA = (a.statusz || "").toLowerCase();
    const statusB = (b.statusz || "").toLowerCase();

    const priorityA = statusPriority[statusA] ?? Number.MAX_SAFE_INTEGER;
    const priorityB = statusPriority[statusB] ?? Number.MAX_SAFE_INTEGER;

    if (priorityA !== priorityB) {
      return priorityA - priorityB;
    }

    return (b.id ?? 0) - (a.id ?? 0);
  });

  const isPickupOrder = (order) => {
    const addressText = (order.cim || "").toLowerCase();
    return addressText.includes("személyes átvétel") || addressText.includes("szemelyes atvetel");
  };

  return (
    <div className="kitchen-container">
      <header className="kitchen-header">
        <h1>🔥 Konyhai Monitor</h1>
        <div className="order-count">Aktív rendelések: {sortedOrders.length}</div>
      </header>

      <div className="kitchen-filter-bar">
        <span className="filter-label">Státusz szűrő:</span>
        <div className="filter-button-group">
          {[
            ["mai", "MAI"],
            ["összes", "Összes"],
            ["folyamatban", "Folyamatban"],
            ["sütés alatt", "Sütés alatt"],
            ["kész", "Kész"],
            ["átvehető", "Átvehető"],
          ].map(([value, label]) => (
            <button
              key={value}
              type="button"
              className={`filter-btn filter-${value.replace(/\s+/g, "-")} ${(value === "mai" ? onlyToday : statusFilter.includes(value)) ? "active" : ""}`}
              onClick={() => toggleStatusFilter(value)}
            >
              {label}
            </button>
          ))}
        </div>
      </div>

      {sortedOrders.length === 0 && (
        <div className="empty-msg">Nincs a kiválasztott szűrésnek megfelelő rendelés. 🍕</div>
      )}

      <div className="kitchen-grid">
        {sortedOrders.map((order) => (
          <div
            key={order.id}
            className={`kitchen-order-card ${order.statusz === "sütés alatt" ? "baking-active" : ""}`}
          >
            <div className="card-top">
              <span className="order-number">#{order.id}</span>
              <span className="order-clock">{order.datum ? order.datum.slice(11, 16) : "--:--"}</span>
            </div>

            <div className="order-info-row">
              <p className={`status-badge ${(order.statusz || "ismeretlen").replace(/\s+/g, "-")}`}>
                {order.statusz || "ismeretlen"}
              </p>
              {isPickupOrder(order) && (
                <p className="pickup-badge">Pultnál átvétel</p>
              )}
            </div>

            <div className="kitchen-items-box">
              <h3>Tételek:</h3>
              {(order.tetelek || []).map((t, i) => (
                <div key={i} className="kitchen-item-row">
                  <div className="item-main">
                    <span className="qty">{t.mennyiseg || 0}x</span>
                    <span className="name">{t.pizzaNev || "Ismeretlen pizza"}</span>
                  </div>
                  {t.feltetek && t.feltetek.length > 0 && (
                    <div className="item-toppings">
                      + {t.feltetek.map((f) => f.nev).join(", ")}
                    </div>
                  )}
                </div>
              ))}
            </div>

            <div className="kitchen-actions">
              {(order.statusz === "folyamatban" || order.statusz === "függő") && (
                <button
                  className="btn-start-baking"
                  onClick={() => startBakingProcess(order.id)}
                >
                  🔥 Sütés Elkezdése (10p)
                </button>
              )}

              {order.statusz === "sütés alatt" && (
                <CountDownTimer
                  initialSeconds={order.sutesiIdo ?? 600}
                  onComplete={() => updateStatus(order.id, "kész")}
                />
              )}

              {(order.statusz === "folyamatban" || order.statusz === "függő" || order.statusz === "sütés alatt") && (
                <button
                  className="btn-instant-ready"
                  onClick={() => markAsReadyImmediately(order.id)}
                >
                  ⚡ Azonnal kész
                </button>
              )}

              {order.statusz === "kész" && (
                <button
                  className="btn-complete"
                  onClick={() => updateStatus(order.id, "átvehető")}
                >
                  ✅ Kész / Átvehető
                </button>
              )}

              {order.statusz === "átvehető" && (
                <div className="ready-text">Pultnál vár kiadásra</div>
              )}
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}

export default KitchenDashboard;
