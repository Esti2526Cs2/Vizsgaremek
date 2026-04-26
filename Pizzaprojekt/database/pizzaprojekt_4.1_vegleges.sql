-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Gép: 127.0.0.1
-- Létrehozás ideje: 2026. Már 30. 18:01
-- Kiszolgáló verziója: 10.4.32-MariaDB
-- PHP verzió: 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Adatbázis: `pizzaprojekt`
--
CREATE DATABASE IF NOT EXISTS `pizzaprojekt` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci;
USE `pizzaprojekt`;

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `feltetek`
--

CREATE TABLE `feltetek` (
  `Feltet_Id` int(11) NOT NULL,
  `Feltet_ar` decimal(10,2) NOT NULL,
  `Nev` varchar(100) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- A tábla adatainak kiíratása `feltetek`
--

INSERT INTO `feltetek` (`Feltet_Id`, `Feltet_ar`, `Nev`) VALUES
(1, 500.00, 'sajt'),
(2, 400.00, 'paprikás szalámi'),
(3, 400.00, 'kockabacon'),
(4, 200.00, 'hegyes erős'),
(5, 200.00, 'cserkópaprika'),
(6, 200.00, 'jalapeno'),
(7, 300.00, 'gomba'),
(8, 400.00, 'sonka'),
(9, 200.00, 'szalámis kockasajt'),
(10, 500.00, 'füstölt sajt'),
(11, 200.00, 'vöröshagyma'),
(12, 200.00, 'zöldpaprika'),
(13, 400.00, 'hagymakarika'),
(14, 300.00, 'mustár'),
(15, 400.00, 'virsli'),
(16, 200.00, 'lilahagyma'),
(17, 300.00, 'spenót'),
(18, 200.00, 'főtt tojás'),
(19, 400.00, 'tarja'),
(20, 200.00, 'csemege uborka'),
(21, 400.00, 'kolbász'),
(22, 300.00, 'kukorica'),
(23, 400.00, 'vegyes zöldség'),
(24, 300.00, 'ananász'),
(25, 400.00, 'brokkoli'),
(26, 500.00, 'lecsó'),
(27, 300.00, 'bab'),
(28, 200.00, 'erős pista'),
(29, 400.00, 'tortilla'),
(30, 200.00, 'torma'),
(31, 300.00, 'fokhagyma'),
(32, 600.00, 'túró');

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `keri`
--

CREATE TABLE `keri` (
  `Cim_Id` int(11) NOT NULL,
  `Rendelo_azonosito` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- A tábla adatainak kiíratása `keri`
--

INSERT INTO `keri` (`Cim_Id`, `Rendelo_azonosito`) VALUES
(101, 201),
(101, 203),
(102, 201),
(103, 202),
(103, 203);

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `kiszallitasi_hely`
--

CREATE TABLE `kiszallitasi_hely` (
  `Cim_Id` int(11) NOT NULL,
  `Varos` varchar(100) NOT NULL,
  `Iranyitoszam` varchar(10) NOT NULL,
  `Utca` varchar(100) NOT NULL,
  `Hazszam` varchar(20) NOT NULL,
  `Emelet_ajto` varchar(50) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- A tábla adatainak kiíratása `kiszallitasi_hely`
--

INSERT INTO `kiszallitasi_hely` (`Cim_Id`, `Varos`, `Iranyitoszam`, `Utca`, `Hazszam`, `Emelet_ajto`) VALUES
(101, 'Szombathely', '9700', 'Fő tér', '1.', 'Fsz. 1.'),
(102, 'Szombathely', '9700', 'Hunyadi János utca', '14.', NULL),
(103, 'Szombathely', '9700', 'Szent Imre herceg utca', '42/A', 'II. em. 20.');

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `lead`
--

CREATE TABLE `lead` (
  `Rendelo_azonosito` int(11) NOT NULL,
  `Rendelesi_Id` int(11) NOT NULL,
  PRIMARY KEY (`Rendelo_azonosito`, `Rendelesi_Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- A tábla adatainak kiíratása `lead`
--

INSERT INTO `lead` (`Rendelo_azonosito`, `Rendelesi_Id`) VALUES
(203, 303);

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `pizza`
--

CREATE TABLE `pizza` (
  `Pizza_Id` int(11) NOT NULL,
  `Pizza_ar` decimal(10,2) NOT NULL,
  `Nev` varchar(100) NOT NULL,
  `Alap` varchar(100) DEFAULT NULL,
  `image` varchar(100) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- A tábla adatainak kiíratása `pizza`
--

INSERT INTO `pizza` (`Pizza_Id`, `Pizza_ar`, `Nev`, `Alap`, `image`) VALUES
(1, 2350.00, 'Mint a villám pizza', 'paradicsomos alap', 'https://res.cloudinary.com/dzseqnbrf/image/upload/v1772562024/1_b4lkbl.png'),
(3, 2450.00, 'Babe pizza', 'paradicsom alap', 'https://res.cloudinary.com/dzseqnbrf/image/upload/v1772562026/3_iknsvg.png'),
(4, 2450.00, 'Transporting pizza', 'paradicsom alap', 'https://res.cloudinary.com/dzseqnbrf/image/upload/v1772562024/4_jgjsj9.png'),
(5, 2450.00, 'Legenda vagyok pizza', 'paradicsom alap', 'https://res.cloudinary.com/dzseqnbrf/image/upload/v1772562020/5_k20vw6.png'),
(6, 2450.00, 'Minden6ó pizza', 'paradicsom alap', 'https://res.cloudinary.com/dzseqnbrf/image/upload/v1772562032/6_vnloru.png'),
(8, 2450.00, 'Pumukli pizza', 'paradicsom alap', 'https://res.cloudinary.com/dzseqnbrf/image/upload/v1772562028/8_epov52.png'),
(9, 2450.00, 'Pearl Harbor pizza', 'paradicsom alap', 'https://res.cloudinary.com/dzseqnbrf/image/upload/v1772562034/9_wyk2ea.png'),
(10, 2550.00, 'Nagymenők pizza', 'paradicsomos alap', 'https://res.cloudinary.com/dzseqnbrf/image/upload/v1772562029/10_xpkhzb.png'),
(11, 2550.00, 'Pancser police pizza', 'Tejfölös alap', 'https://res.cloudinary.com/dzseqnbrf/image/upload/v1772562029/11_cqkud5.png'),
(12, 2650.00, 'Mad Max pizza', 'paradicsomos alap', 'https://res.cloudinary.com/dzseqnbrf/image/upload/v1772562031/12_qmcjih.png'),
(13, 2650.00, 'Blöff pizza', 'paradicsomos alap', 'https://res.cloudinary.com/dzseqnbrf/image/upload/v1772562029/13_uc1e3y.png'),
(14, 2450.00, 'Beépített szépség 1. pizza', 'paradicsomos alap', 'https://res.cloudinary.com/dzseqnbrf/image/upload/v1772562030/14_tqcbhf.png'),
(15, 2450.00, 'Beépített szépség 2. pizza', 'paradicsomos alap', 'https://res.cloudinary.com/dzseqnbrf/image/upload/v1772562030/15_eiwh32.png'),
(16, 2650.00, 'Ananász expressz 1. pizza', 'paradicsomos alap', 'https://res.cloudinary.com/dzseqnbrf/image/upload/v1772562031/16_n38thz.png'),
(17, 2650.00, 'Ananász expressz 2. pizza', 'paradicsomos alap', 'https://res.cloudinary.com/dzseqnbrf/image/upload/v1772562032/17_fdhfjw.png'),
(18, 2450.00, 'Förtelmes főnökök 1. pizza', 'paradicsomos alap', 'https://res.cloudinary.com/dzseqnbrf/image/upload/v1772562032/18_eww43w.png'),
(19, 2450.00, 'Förtelmes főnökök 2. pizza', 'paradicsomos alap', 'https://res.cloudinary.com/dzseqnbrf/image/upload/v1772562033/19_d9ew4k.png'),
(20, 2550.00, 'Bosszúállók', 'erős pista-paradicsom alap', 'https://res.cloudinary.com/dzseqnbrf/image/upload/v1772562034/20_l8ufd4.png'),
(21, 2450.00, 'Argo 1. pizza', 'paradicsom alap', 'https://res.cloudinary.com/dzseqnbrf/image/upload/v1772562034/21_gnrefr.png'),
(22, 2450.00, 'Argo 2. pizza', 'paradicsom alap', 'https://res.cloudinary.com/dzseqnbrf/image/upload/v1772562033/22_lmybrg.png'),
(23, 2550.00, 'Cápa 1. pizza', 'paradicsom alap', 'https://res.cloudinary.com/dzseqnbrf/image/upload/v1772562035/23_ghna5w.png'),
(24, 2550.00, 'Cápa 2. pizza', 'paradicsom alap', 'https://res.cloudinary.com/dzseqnbrf/image/upload/v1772562035/24_mnql5f.png'),
(25, 2550.00, 'Karib tenger kalózai pizza', 'paradicsom alap', 'https://res.cloudinary.com/dzseqnbrf/image/upload/v1772562035/25_obrnoz.png'),
(26, 2550.00, 'Jó reggelt Vietnám pizza', 'paradicsom alap', 'https://res.cloudinary.com/dzseqnbrf/image/upload/v1772562035/26_ffpnav.png'),
(27, 2550.00, 'Taxi pizza', 'Tejfölös alap', 'https://res.cloudinary.com/dzseqnbrf/image/upload/v1772562047/27_wv8xfh.png'),
(28, 2550.00, 'Francia kapcsolat', 'paradicsom alap', 'https://res.cloudinary.com/dzseqnbrf/image/upload/v1772562036/28_jhtdbo.png'),
(30, 2550.00, 'Jó barátok pizza', 'paradicsom alap', 'https://res.cloudinary.com/dzseqnbrf/image/upload/v1772562037/30_mbx0pc.png'),
(31, 2550.00, 'Három testőr pizza', 'paradicsom alap', 'https://res.cloudinary.com/dzseqnbrf/image/upload/v1772562042/31_zvxilc.png'),
(32, 2450.00, 'Lecsó pizza', 'Tejfölös alap', 'https://res.cloudinary.com/dzseqnbrf/image/upload/v1772562043/32_eabp9j.png'),
(33, 2550.00, 'Az ördög jobb és bal keze pizza', 'paradicsom alap', 'https://res.cloudinary.com/dzseqnbrf/image/upload/v1772562042/33_vezos0.png'),
(34, 2550.00, 'Különben dühbe jövünk pizza', 'paradicsom alap', 'https://res.cloudinary.com/dzseqnbrf/image/upload/v1772562039/34_syzeya.png'),
(35, 2750.00, 'Megint dühbe jövünk pizza', 'Para csípős alap', 'https://res.cloudinary.com/dzseqnbrf/image/upload/v1772562042/35_capvos.png'),
(36, 2550.00, 'Legényanya pizza', 'Tejfölös alap', 'https://res.cloudinary.com/dzseqnbrf/image/upload/v1772562042/36_frtsxb.png'),
(37, 2450.00, 'Tűzhányó pizza', 'paradicsom alap', 'https://res.cloudinary.com/dzseqnbrf/image/upload/v1772562046/37_fnwljm.png'),
(38, 2550.00, 'Szökés pizza', 'Tejfölös alap', 'https://res.cloudinary.com/dzseqnbrf/image/upload/v1772562055/38_cnylpb.png'),
(39, 2550.00, 'Sziget pizza', 'paradicsom alap', 'https://res.cloudinary.com/dzseqnbrf/image/upload/v1772562053/39_b92v6y.png'),
(40, 2550.00, 'Dante pokla pizza', 'paradicsom alap', 'https://res.cloudinary.com/dzseqnbrf/image/upload/v1772562046/40_hsnqc3.png'),
(41, 2550.00, 'Fantasztikus négyes pizza', 'paradicsom alap', 'https://res.cloudinary.com/dzseqnbrf/image/upload/v1772562052/41_nilkbm.png'),
(42, 2550.00, 'Szikla pizza', 'paradicsom alap', 'https://res.cloudinary.com/dzseqnbrf/image/upload/v1772562050/42_l7l1ir.png'),
(43, 2550.00, 'Szövetség pizza', 'paradicsom alap', 'https://res.cloudinary.com/dzseqnbrf/image/upload/v1772562063/43_odwbt2.png'),
(44, 2550.00, 'Alkonyattól pirkadatig pizza', 'paradicsom alap', 'https://res.cloudinary.com/dzseqnbrf/image/upload/v1772562052/44_taefhz.png'),
(45, 2550.00, 'Magyar vándor pizza', 'Tejfölös alap', 'https://res.cloudinary.com/dzseqnbrf/image/upload/v1772562053/45_wokhdv.png'),
(46, 2550.00, 'Space Jam pizza', 'Tejfölös alap', 'https://res.cloudinary.com/dzseqnbrf/image/upload/v1772562055/46_e4xxra.png'),
(47, 2550.00, 'Roger nyúl pizza', 'Tejfölös alap', 'https://res.cloudinary.com/dzseqnbrf/image/upload/v1772562062/47_owhziz.png'),
(48, 2750.00, 'Mexikói pizza', 'paradicsom alap', 'https://res.cloudinary.com/dzseqnbrf/image/upload/v1772562057/48_p9637f.png'),
(49, 2550.00, 'Négy szoba pizza', 'paradicsom alap', 'https://res.cloudinary.com/dzseqnbrf/image/upload/v1772562062/49_rxo0p0.png'),
(50, 2550.00, 'Terminátor pizza', 'paradicsom alap', 'https://res.cloudinary.com/dzseqnbrf/image/upload/v1772562058/50_k26byv.png'),
(51, 2550.00, 'Zorró álarca pizza', 'paradicsom alap', 'https://res.cloudinary.com/dzseqnbrf/image/upload/v1772562066/51_hxd41a.png'),
(52, 2550.00, 'Indul a bakterház pizza', 'Tejfölös alap', 'https://res.cloudinary.com/dzseqnbrf/image/upload/v1772562060/52_wxksru.png'),
(53, 2550.00, 'Üvegtigris 1. pizza', 'Tejfölös alap', 'https://res.cloudinary.com/dzseqnbrf/image/upload/v1772562061/53_iaphld.png'),
(54, 2550.00, 'Üvegtigris 2. pizza', 'Kapros tejfölös alap', 'https://res.cloudinary.com/dzseqnbrf/image/upload/v1772562064/54_vnzseu.png'),
(55, 2550.00, 'Tüskevár pizza', 'Tejfölös alap', 'https://res.cloudinary.com/dzseqnbrf/image/upload/v1772562023/2_ufbldt.png'),
(56, 2650.00, 'Shrek pizza', 'spenót-tejföl alap', 'https://res.cloudinary.com/dzseqnbrf/image/upload/v1772562066/56_icykcy.png'),
(57, 2650.00, 'Wasabi pizza', 'mustár-tejföl alap', 'https://res.cloudinary.com/dzseqnbrf/image/upload/v1772562074/57_eb1jod.png'),
(58, 2650.00, 'Made in Hungária pizza', 'libatepertőkrém alap', 'https://res.cloudinary.com/dzseqnbrf/image/upload/v1772562068/58_inimar.png'),
(59, 2650.00, 'Amerikába jöttem pizza', 'BBQ alap', 'https://res.cloudinary.com/dzseqnbrf/image/upload/v1772562075/59_nco25r.png'),
(60, 2750.00, 'Papírkutyák pizza', 'Sajtkrémes alap', 'https://res.cloudinary.com/dzseqnbrf/image/upload/v1772562067/60_ygnaus.png'),
(61, 2750.00, 'Csinibaba pizza', 'Sajtkrémes alap', 'https://res.cloudinary.com/dzseqnbrf/image/upload/v1772562069/61_kfdbub.png'),
(62, 2650.00, 'Meseautó pizza', 'tejfölös alap', 'https://res.cloudinary.com/dzseqnbrf/image/upload/v1772562074/62_d7zawb.png'),
(63, 2650.00, 'Feláldozhatók pizza', 'erős pista-paradicsom alap', 'https://res.cloudinary.com/dzseqnbrf/image/upload/v1772562072/63_lsxusq.png'),
(64, 2450.00, 'Másnaposok pizza', 'paradicsom alap', 'https://res.cloudinary.com/dzseqnbrf/image/upload/v1772562073/64_rro1rz.png'),
(65, 2550.00, 'Másnaposok 2. pizza', 'paradicsom alap', 'https://res.cloudinary.com/dzseqnbrf/image/upload/v1772562156/65_enhkqi.png'),
(66, 2750.00, 'Másnaposok 3. pizza', 'paradicsom alap', 'https://res.cloudinary.com/dzseqnbrf/image/upload/v1772562020/66_qpzpmt.png'),
(67, 2750.00, 'Olasz meló', 'paradicsom alap', 'https://res.cloudinary.com/dzseqnbrf/image/upload/v1772562023/67_mt12pe.png'),
(68, 2550.00, 'Római vakáció pizza', 'paradicsom alap', 'https://res.cloudinary.com/dzseqnbrf/image/upload/v1772562027/68_zvxbmx.png'),
(69, 2550.00, 'Minden lében 2 kanál pizza', 'Tejfölös alap', 'https://res.cloudinary.com/dzseqnbrf/image/upload/v1772562023/69_ubzd3n.png'),
(70, 2550.00, 'Nincs kettő négy nélkül pizza', 'paradicsom alap', 'https://res.cloudinary.com/dzseqnbrf/image/upload/v1772562024/70_ks8dkq.png');

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `pizza_feltetek`
--

CREATE TABLE `pizza_feltetek` (
  `Pizza_Id` int(11) NOT NULL,
  `Feltet_Id` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- A tábla adatainak kiíratása `pizza_feltetek`
--

INSERT INTO `pizza_feltetek` (`Pizza_Id`, `Feltet_Id`) VALUES
(1, 1),
(3, 1),
(3, 7),
(3, 8),
(4, 1),
(4, 5),
(4, 7),
(4, 8),
(4, 22),
(5, 1),
(5, 3),
(5, 8),
(5, 10),
(6, 1),
(6, 5),
(6, 7),
(6, 8),
(8, 1),
(8, 7),
(8, 8),
(8, 22),
(9, 1),
(9, 3),
(9, 12),
(9, 16),
(9, 22),
(10, 1),
(10, 2),
(10, 8),
(11, 1),
(11, 8),
(11, 18),
(11, 20),
(12, 1),
(12, 3),
(12, 4),
(12, 13),
(12, 21),
(13, 1),
(13, 3),
(13, 8),
(13, 22),
(14, 1),
(14, 23),
(15, 1),
(15, 8),
(15, 23),
(16, 1),
(16, 8),
(16, 22),
(16, 24),
(17, 1),
(17, 7),
(17, 8),
(17, 24),
(18, 1),
(18, 2),
(18, 22),
(19, 1),
(19, 2),
(19, 7),
(20, 1),
(20, 2),
(20, 4),
(20, 13),
(21, 1),
(21, 8),
(21, 22),
(22, 1),
(22, 2),
(22, 22),
(23, 1),
(23, 11),
(24, 1),
(24, 8),
(24, 11),
(25, 1),
(25, 7),
(25, 8),
(25, 22),
(26, 1),
(26, 7),
(26, 11),
(26, 12),
(27, 1),
(27, 25),
(28, 1),
(28, 7),
(28, 8),
(28, 22),
(28, 24),
(30, 1),
(30, 7),
(30, 8),
(30, 12),
(30, 22),
(31, 1),
(31, 3),
(31, 7),
(31, 8),
(32, 1),
(32, 15),
(32, 26),
(33, 1),
(33, 2),
(33, 7),
(33, 11),
(34, 1),
(34, 2),
(34, 7),
(34, 8),
(35, 1),
(35, 2),
(35, 3),
(35, 4),
(35, 10),
(35, 28),
(36, 1),
(36, 2),
(36, 4),
(36, 27),
(37, 1),
(37, 4),
(37, 5),
(37, 8),
(38, 1),
(38, 7),
(38, 8),
(38, 16),
(38, 28),
(39, 1),
(39, 8),
(39, 24),
(40, 1),
(40, 2),
(40, 6),
(40, 8),
(40, 28),
(41, 1),
(41, 2),
(41, 7),
(41, 8),
(41, 22),
(42, 1),
(42, 8),
(42, 11),
(43, 1),
(43, 3),
(43, 10),
(43, 11),
(44, 1),
(44, 3),
(44, 8),
(44, 10),
(44, 11),
(45, 1),
(45, 3),
(45, 16),
(45, 32),
(46, 1),
(46, 3),
(46, 8),
(46, 29),
(47, 1),
(47, 3),
(47, 8),
(47, 18),
(47, 30),
(48, 1),
(48, 6),
(48, 11),
(48, 22),
(48, 27),
(49, 1),
(49, 3),
(49, 7),
(49, 8),
(49, 22),
(50, 1),
(50, 7),
(50, 8),
(50, 19),
(51, 1),
(51, 2),
(51, 11),
(52, 1),
(52, 2),
(52, 3),
(52, 5),
(52, 16),
(52, 31),
(53, 1),
(53, 3),
(53, 18),
(53, 20),
(54, 1),
(54, 15),
(54, 19),
(54, 20),
(55, 1),
(55, 7),
(55, 8),
(55, 17),
(55, 31),
(56, 1),
(56, 11),
(56, 17),
(56, 18),
(57, 1),
(57, 3),
(57, 14),
(57, 15),
(57, 16),
(58, 1),
(58, 3),
(58, 12),
(58, 13),
(59, 3),
(59, 10),
(59, 11),
(60, 1),
(60, 3),
(60, 4),
(60, 22),
(61, 1),
(61, 2),
(62, 1),
(62, 7),
(62, 8),
(62, 9),
(63, 1),
(63, 2),
(63, 3),
(63, 4),
(63, 5),
(63, 6),
(64, 1),
(64, 7),
(64, 8),
(64, 31),
(65, 1),
(65, 11),
(65, 21),
(65, 28),
(66, 1),
(66, 3),
(66, 10),
(66, 17),
(66, 31),
(67, 1),
(67, 2),
(67, 3),
(67, 5),
(67, 10),
(68, 1),
(68, 8),
(68, 18),
(68, 25),
(69, 1),
(69, 8),
(69, 10),
(69, 17),
(70, 1),
(70, 2),
(70, 3),
(70, 8),
(70, 10);

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `rendeles`
--

CREATE TABLE `rendeles` (
  `Rendelesi_Id` int(11) NOT NULL,
  `Mennyiseg` int(11) NOT NULL,
  `Datum_ido` timestamp NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp(),
  `Fizetesi_mod` varchar(50) NOT NULL,
  `Statusz` varchar(50) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- A tábla adatainak kiíratása `rendeles`
--

INSERT INTO `rendeles` (`Rendelesi_Id`, `Mennyiseg`, `Datum_ido`, `Fizetesi_mod`, `Statusz`) VALUES
(301, 2, '2026-03-30 15:14:14', 'Bankkártya', 'Kiszállítva'),
(302, 1, '2026-03-30 15:14:37', 'Készpénz', 'Feldolgozás alatt'),
(303, 3, '2026-03-30 15:14:47', 'SZÉP-kártya', 'Kiszállítás alatt'),
(304, 0, '2026-03-30 13:43:08', 'keszpenz', 'folyamatban');

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `tartalmaz`
--

CREATE TABLE `tartalmaz` (
  `Rendelesi_Id` int(11) NOT NULL,
  `Pizza_Id` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- A tábla adatainak kiíratása `tartalmaz`
--

INSERT INTO `tartalmaz` (`Rendelesi_Id`, `Pizza_Id`) VALUES
(301, 10),
(301, 62),
(302, 63);

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `vasarlo`
--

CREATE TABLE `vasarlo` (
  `Rendelo_azonosito` int(11) NOT NULL,
  `Nev` varchar(150) NOT NULL,
  `Telefonszam` varchar(20) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- A tábla adatainak kiíratása `vasarlo`
--

INSERT INTO `vasarlo` (`Rendelo_azonosito`, `Nev`, `Telefonszam`) VALUES
(1, 'Gubacsi Tamás', '06304946141'),
(201, 'Kovács Péter', '+36201234567'),
(202, 'Nagy Anna', '+36309876543'),
(203, 'Kiss Dániel', '+36705551122'),
(204, 'Gipsz Jakab', '7826348'),
(205, 'kiss Pista', '23423452'),
(206, 'kiss Pistasdfa', '23423452'),
(207, 'mshdfkjahjksdh', '23423452'),
(208, 'Jakab Péter', '12341324');

--
-- Indexek a kiírt táblákhoz
--

--
-- A tábla indexei `feltetek`
--
ALTER TABLE `feltetek`
  ADD PRIMARY KEY (`Feltet_Id`);

--
-- A tábla indexei `keri`
--
ALTER TABLE `keri`
  ADD PRIMARY KEY (`Cim_Id`,`Rendelo_azonosito`),
  ADD KEY `Rendelő_azonosító` (`Rendelo_azonosito`);

--
-- A tábla indexei `kiszallitasi_hely`
--
ALTER TABLE `kiszallitasi_hely`
  ADD PRIMARY KEY (`Cim_Id`);

--
-- A tábla indexei `lead`
--
ALTER TABLE `lead`
  ADD KEY `Rendelési_Id` (`Rendelesi_Id`);

--
-- A tábla indexei `pizza`
--
ALTER TABLE `pizza`
  ADD PRIMARY KEY (`Pizza_Id`);

--
-- A tábla indexei `pizza_feltetek`
--
ALTER TABLE `pizza_feltetek`
  ADD PRIMARY KEY (`Pizza_Id`,`Feltet_Id`),
  ADD KEY `Feltet_Id` (`Feltet_Id`);

--
-- A tábla indexei `rendeles`
--
ALTER TABLE `rendeles`
  ADD PRIMARY KEY (`Rendelesi_Id`);

--
-- A tábla indexei `tartalmaz`
--
ALTER TABLE `tartalmaz`
  ADD PRIMARY KEY (`Rendelesi_Id`,`Pizza_Id`),
  ADD KEY `Pizza_Id` (`Pizza_Id`);

--
-- A tábla indexei `vasarlo`
--
ALTER TABLE `vasarlo`
  ADD PRIMARY KEY (`Rendelo_azonosito`);

--
-- A kiírt táblák AUTO_INCREMENT értéke
--

--
-- AUTO_INCREMENT a táblához `kiszallitasi_hely`
--
ALTER TABLE `kiszallitasi_hely`
  MODIFY `Cim_Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=104;

--
-- AUTO_INCREMENT a táblához `rendeles`
--
ALTER TABLE `rendeles`
  MODIFY `Rendelesi_Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=305;

--
-- AUTO_INCREMENT a táblához `vasarlo`
--
ALTER TABLE `vasarlo`
  MODIFY `Rendelo_azonosito` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=209;

--
-- Megkötések a kiírt táblákhoz
--

--
-- Megkötések a táblához `keri`
--
ALTER TABLE `keri`
  ADD CONSTRAINT `keri_ibfk_1` FOREIGN KEY (`Cim_Id`) REFERENCES `kiszallitasi_hely` (`Cim_Id`),
  ADD CONSTRAINT `keri_ibfk_2` FOREIGN KEY (`Rendelo_azonosito`) REFERENCES `vasarlo` (`Rendelo_azonosito`);

--
-- Megkötések a táblához `lead`
--
ALTER TABLE `lead`
  ADD CONSTRAINT `lead_ibfk_1` FOREIGN KEY (`Rendelo_azonosito`) REFERENCES `vasarlo` (`Rendelo_azonosito`),
  ADD CONSTRAINT `lead_ibfk_2` FOREIGN KEY (`Rendelesi_Id`) REFERENCES `rendeles` (`Rendelesi_Id`);

--
-- Megkötések a táblához `pizza_feltetek`
--
ALTER TABLE `pizza_feltetek`
  ADD CONSTRAINT `pizza_feltetek_ibfk_1` FOREIGN KEY (`Pizza_Id`) REFERENCES `pizza` (`Pizza_Id`),
  ADD CONSTRAINT `pizza_feltetek_ibfk_2` FOREIGN KEY (`Feltet_Id`) REFERENCES `feltetek` (`Feltet_Id`);

--
-- Megkötések a táblához `tartalmaz`
--
ALTER TABLE `tartalmaz`
  ADD CONSTRAINT `tartalmaz_ibfk_1` FOREIGN KEY (`Rendelesi_Id`) REFERENCES `rendeles` (`Rendelesi_Id`),
  ADD CONSTRAINT `tartalmaz_ibfk_2` FOREIGN KEY (`Pizza_Id`) REFERENCES `pizza` (`Pizza_Id`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
