# Auto Folder Sync

[English](#english) | [한국어](#한국어)

---
<img width="888" height="644" alt="image" src="https://github.com/user-attachments/assets/c357fdd0-2226-41ca-862a-468ff25c4e96" />


<a name="한국어"></a>
## 🇰🇷 Auto Folder Sync (자동 폴더 동기화 프로그램)

산업용 장비 제어 PC 및 일반 데스크톱 환경에서 지정한 원본 폴더의 데이터를 백업 폴더로 주기적으로 자동 동기화하여 데이터 유실을 방지하는 .NET 8 WPF 기반의 경량 데스크톱 애플리케이션입니다.

### 💡 개발 배경 및 필요성
- **RAID의 한계 극복**: 하드웨어 미러링(RAID 1)은 물리적 드라이브 고장에는 대응할 수 있으나, 랜섬웨어 감염이나 작업자 실수로 인한 파일 손상 시 오염된 상태가 실시간으로 동기화되어 백업의 의미가 사라지는 한계가 있습니다.
- **산업용 장비 실무 적용**: 장기 구동되는 산업용 장비 환경에서 로그 및 핵심 데이터가 담긴 특정 폴더를 시간차를 두고 안전하게 백업하기 위해 개발되었습니다. 소프트웨어적인 주기적 미러링을 통해 소프트웨어적 위협과 파일 손상에 대비합니다.

### 🚀 주요 기능
- **완벽한 폴더 미러링**: 원본 폴더의 파일 생성·수정·삭제 변경 사항을 지정한 대상 폴더에 안전하게 동기화합니다.
- **자유로운 백업 주기 설정**: 1분, 10분, 30분, 1시간 단위로 자동 백업 간격을 유연하게 선택할 수 있습니다.
- **장기 구동 안정성 (로그 자동 정리)**: 산업용 PC에서 24시간 무인 구동 시 발생할 수 있는 로그 비대화 방지를 위해, 내부 프로그램 로그는 **항상 가장 최신 1,000줄만 유지**되도록 최적화되어 있습니다[cite: 16].
- **다국어 지원**: 한국어(KO)와 영어(EN) 인터페이스를 실시간으로 전환할 수 있습니다.
- **윈도우 시작 시 자동 실행**: 시스템 부팅 시 백그라운드에서 조용하고 확실하게 데이터를 보호합니다.

### 💻 시스템 요구 사항
- **지원 환경**: Windows 10 및 Windows 11 (64비트) (`.NET 8` 기반 Self-contained)
- **미지원 환경**: Windows 7 및 32비트 운영체제

## ⚖️ 라이선스 및 이용 약관 (Terms of Use)

* **비영리 개인 사용 허용:** 개인적인 용도나 비영리 목적의 환경에서는 자유롭게 다운로드하여 사용할 수 있습니다.
* **상업적 이용 금지:** 기업, 단체, 상업적 매장, 영리 목적의 프로젝트 등 **어떠한 형태의 상업적 이용도 엄격히 금지**합니다. (상업적 이용을 원하실 경우 별도 문의바랍니다.)
* **소스코드 비공개 및 역공학 금지:** 본 프로그램의 소스코드는 공개되지 않으며, 사용자는 프로그램의 디컴파일(Decompile), 리버스 엔지니어링(Reverse Engineering), 무단 수정 및 재배포를 할 수 없습니다.
* **면책 조항:** 본 프로그램을 사용하면서 발생하는 어떠한 시스템 손실이나 문제에 대해서도 개발자는 책임을 지지 않습니다.
 

---
<img width="888" height="644" alt="image" src="https://github.com/user-attachments/assets/dcd1a5f0-9b8c-43ab-8c68-4ecfaf1f7876" />


<a name="english"></a>
## 🇬🇧 Auto Folder Sync

A lightweight .NET 8 WPF desktop application designed to automatically synchronize and protect data from a specified source folder to a backup folder, optimized for both industrial control systems and general use.

### 💡 Background & Motivation
- **Overcoming RAID Limitations**: While hardware mirroring (RAID 1) protects against physical drive failures, it instantly propagates corrupted states (due to ransomware or human error) to the backup drive. 
- **Industrial Field Application**: Developed to reliably back up specific target folders (such as dispenser control logs and working files) running on long-term industrial systems using scheduled software-based time-diff mirroring rather than raw real-time replication.

### 🚀 Key Features
- **Folder Mirroring**: Accurately synchronizes file additions, modifications, and deletions from the source to the destination folder.
- **Flexible Backup Intervals**: Choose from 1-minute, 10-minute, 30-minute, or 1-hour automatic backup intervals.
- **Long-term Stability (Log Trimming)**: Automatically manages and retains only the **latest 1,000 log lines** to prevent disk/memory bloat during 24/7 unmonitored industrial operation[cite: 16].
- **Multi-language Support**: Real-time switching between Korean (KO) and English (EN) interfaces.
- **Windows Startup Integration**: Option to run automatically upon system startup in the background.

### 💻 System Requirements
- **Supported**: Windows 10 & Windows 11 (64-bit, .NET 8 Self-contained)
- **Not Supported**: Windows 7 and 32-bit OS

---

## 📄 License and Terms of Use
Non-Profit Personal Use Allowed: You are free to download and use this program for personal or non-profit environments.
Commercial Use Prohibited: Any form of commercial use, including use by corporations, organizations, commercial establishments, or profit-seeking projects, is strictly prohibited. (Please contact the developer separately for commercial licensing inquiries.)
Source Code Secrecy & Anti-Reverse Engineering: The source code of this program is not disclosed. Users are strictly prohibited from decompiling, reverse engineering, modifying, or redistributing the software without authorization.
Disclaimer: The developer assumes no liability or responsibility for any system loss or issues that may arise from using this program.
