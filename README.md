# 📂 Auto Folder Sync

<div align="center">

[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](https://opensource.org/licenses/MIT)
[![Platform](https://img.shields.io/badge/Platform-Windows%2010%20%2F%2011%20(64-bit)-lightgrey)](https://www.microsoft.com/windows)
[![Framework](https://img.shields.io/badge/.NET-8.0-purple)](https://dotnet.microsoft.com/)

A lightweight, high-stability automated folder synchronization and backup utility designed for Windows, specialized for multi-drive environments (e.g., C: to D: drive mirroring).

[Korean README](#-한국어-소개) | [English README](#-english-introduction)

</div>

---
<img width="886" height="643" alt="image" src="https://github.com/user-attachments/assets/271e9978-1d44-4563-a2fe-663cc1529152" />

## 🇰🇷 한국어 소개

산업용 장비 제어 PC나 일반 데스크톱 환경에서 예기치 못한 디스크 오류나 조작 실수로 인한 데이터 유실을 방지하기 위해 개발된 가벼운 오픈소스 백업 유틸리티입니다.

### 💡 Why Auto Folder Sync? (개발 배경 및 필요성)
* **2개 이상의 드라이브 환경에 최적화:** C드라이브(시스템/작업 영역)의 핵심 데이터를 D드라이브(백업 영역)로 안전하게 격리·복제(`C: ➔ D:`)할 때 가장 강력한 효과를 발휘합니다.
* **RAID의 한계 극복 (랜섬웨어 및 실수 대비):** 실시간 동기화 프로그램은 랜섬웨어 감염이나 파일 삭제 실수가 발생할 경우 백업본까지 즉시 오염됩니다. 반면, 본 프로그램은 **시간차 주기적 미러링** 방식을 사용하여 손상 전의 안전한 백업본을 복구용으로 보존합니다.

### 🚀 주요 기능 (Key Features)
1. **시간차 주기적 미러링:** 1분, 10분, 30분, 1시간 주기로 원본 폴더의 파일 생성·수정·삭제 내역을 대상 폴더에 정확히 동기화합니다.
2. **장기 구동 안정성 (로그 자동 관리):** 24시간 무인 구동되는 산업용 환경을 고려하여, 내부 로그 파일(`backup.log`)이 항상 **최신 1,000줄만 유지**되도록 설계되어 디스크 용량 낭비가 없습니다.
3. **윈도우 시작 시 자동 실행:** 부팅과 동시에 백그라운드에서 조용히 실행되며, 시작 시 곧바로 자동 백업이 구동됩니다.
4. **실시간 다국어 지원:** 한국어(KO)와 영어(EN) 인터페이스를 실시간으로 전환할 수 있습니다.

### 📦 시스템 요구 사항 (System Requirements)
* **OS:** Windows 10 / Windows 11 (64-bit only)
* **Runtime:** .NET 8.0 Desktop Runtime
* **Storage:** 설치 공간 50MB 미만 / 로그 및 설정 파일(`AppData`) 수 MB 이내

---
<img width="886" height="643" alt="image" src="https://github.com/user-attachments/assets/08f13919-93c1-4fcd-8ee6-e8e60d422ce3" />

## 🇬🇧 English Introduction

A lightweight, open-source automated folder synchronization and backup utility for Windows, optimized for multi-drive setups (e.g., mirroring data from C: drive to D: drive).

### 💡 Why Auto Folder Sync?
* **Optimized for Multi-Drive Environments:** Delivers maximum reliability when separating and backing up critical data from C: drive to a secondary D: drive or partition.
* **Overcoming RAID Limitations:** Real-time sync tools instantly propagate ransomware infections or accidental deletions to the backup. `Auto Folder Sync` uses a **time-delayed periodic mirroring** approach, preserving a safe historical backup before damage occurs.

### 🚀 Key Features
1. **Periodic Mirroring with Time Delay:** Synchronizes file creations, modifications, and deletions at user-defined intervals (1 min, 10 min, 30 min, 1 hour).
2. **Long-term Stability (Auto-Trim Logs):** Automatically maintains only the latest 1,000 lines in `backup.log` to prevent disk bloat during 24/7 unattended industrial operation.
3. **Windows Startup Integration:** Runs quietly in the background upon system boot and initiates automatic backup immediately.
4. **Real-time Localization:** Instantly switch between Korean (KO) and English (EN) interfaces.

### 📦 System Requirements
* **OS:** Windows 10 / Windows 11 (64-bit)
* **Runtime:** .NET 8.0 Desktop Runtime
* **Storage:** < 50 MB for installation, minimal footprint for logs & settings (`AppData`).

---

## 🛠️ Installation & Usage (사용 방법)

1. **Download:** Go to the [Releases](https://github.com/eddiesweet/IndustrialBackup/releases) tab and download the latest setup installer (`AutoFolderSync_Setup_vX.X.X.exe`).
2. **Configure Paths:** Select your **Sync Source** folder (e.g., working directory) and **Sync Target** folder (e.g., backup drive).
3. **Set Options:** Enable automatic backup, choose your preferred interval, and optionally enable Windows startup execution.
4. **Log & Data Location:** 
   * Settings: `%localappdata%\AutoFolderSync\settings.json`
   * Logs: `%localappdata%\AutoFolderSync\Logs\backup.log`

---

## 📄 License

This project is licensed under the **MIT License**. See the [LICENSE](LICENSE) file for details.
