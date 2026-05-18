# ?? KitchenFresh - 스마트 식재료 관리 앱

> 유통기한 관리, OCR 자동 인식, 레시피 추천까지 한번에!

---

## ?? 프로젝트 소개

KitchenFresh 는 냉장고 속 식재료를 스마트하게 관리하는 **Windows WinForms 앱**이에요.

카메라 또는 사진으로 식품 포장지를 찍으면 **Google Vision API** 가 유통기한을 자동으로 인식하고,
유통기한이 임박한 식재료가 있으면 앱 실행 시 자동으로 알림을 주고,
**식품의약안전처 레시피 DB** 에서 임박한 식재료로 만들 수 있는 음식을 추천해줘요.

---

## ? 주요 기능

### ?? 식재료 관리
- 카메라 촬영 또는 이미지 업로드로 식재료 추가
- Google Vision API OCR 로 유통기한 자동 인식
- 카테고리별 식재료 목록 표시
- 신선도 바로 남은 기간 시각화
- 식재료 삭제 기능

### ?? 유통기한 알림
- 앱 실행 시 임박(1~3일) 및 만료 식재료 자동 체크
- 만료/임박 식재료 목록 표시

### ? 레시피 추천
- 임박한 식재료 기반 레시피 자동 검색
- 식품의약안전처 조리식품 레시피 DB 연동
- 재료, 칼로리, 조리순서 상세 표시

### ?? 폐기 가이드
- 카테고리별 올바른 음식물 폐기 방법 안내
- 음식물쓰레기 / 일반쓰레기 / 재활용 분류

### ?? 외국 식품 지원
- 외국 식품 모드 ON/OFF 토글
- 일/월/년 형식 날짜 자동 변환

---

## ??? 사용 기술

| 기술 | 용도 |
|------|------|
| C# WinForms | UI 개발 |
| Google Cloud Vision API | OCR 텍스트 인식 |
| 식품의약안전처 레시피 DB | 레시피 추천 |
| MySQL | 식재료 데이터 저장 |
| AForge.Video.DirectShow | 카메라 촬영 |
| Tesseract OCR | 초기 OCR (Google Vision 으로 교체) |
| Newtonsoft.Json | JSON 파싱 |
| Microsoft.Extensions.Configuration | 설정 파일 관리 |

---

## ?? 프로젝트 구조

```
ExFood/
├── Controls/              ← 커스텀 UI 컴포넌트
│   ├── RoundedPanel.cs    ← 둥근 모서리 패널
│   ├── FreshnessBar.cs    ← 신선도 진행 바
│   └── FoodItemCard.cs    ← 식재료 카드
│
├── Models/                ← 데이터 구조
│   ├── FoodItem.cs        ← 식재료 데이터 모델
│   └── AppConfig.cs       ← 설정값 읽기
│
├── Services/              ← 비즈니스 로직
│   ├── OcrService.cs      ← Google Vision OCR
│   ├── DbService.cs       ← MySQL CRUD
│   ├── CategoryService.cs ← 카테고리 관리
│   ├── RecipeService.cs   ← 레시피 API
│   └── AlarmService.cs    ← 유통기한 알림
│
├── Forms/                 ← 화면
│   ├── AddFoodForm.cs     ← 식재료 추가
│   ├── CameraForm.cs      ← 카메라 촬영
│   ├── AlarmForm.cs       ← 알림 화면
│   ├── RecipeForm.cs      ← 레시피 목록
│   ├── RecipeDetailForm.cs← 레시피 상세
│   └── DisposalGuideForm.cs← 폐기 가이드
│
├── Form1.cs               ← 메인 화면
├── appsettings.json       ← 설정 파일 (git 제외)
└── .gitignore
```

---

## ?? 설치 및 실행

### 1. 프로젝트 Clone

```bash
git clone https://github.com/your-repo/ExFood.git
```

### 2. NuGet 패키지 설치

```
PM> Install-Package Google.Cloud.Vision.V1
PM> Install-Package Newtonsoft.Json
PM> Install-Package MySql.Data
PM> Install-Package Microsoft.Extensions.Configuration
PM> Install-Package Microsoft.Extensions.Configuration.Json
PM> Install-Package AForge.Video.DirectShow
```

### 3. appsettings.json 생성

프로젝트 루트에 `appsettings.json` 파일 생성:

```json
{
  "ApiKeys": {
    "GoogleVision": "구글_Vision_API_키",
    "FoodSafety": "식품안전처_인증키"
  },
  "Database": {
    "Server": "aws-1-ap-northeast-2.pooler.supabase.com",
    "Port": "5432",
    "User": "postgres.xfcyuxmifmipgizvbmqr",
    "Password": "나의__비밀번호",
    "Name": "postgres"
  }
}
```

### 4. MySQL 테이블 생성

```sql
CREATE DATABASE food_manager;

USE food_manager;

CREATE TABLE ingredients (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(100) NOT NULL,
    Category VARCHAR(100),
    StoragePlace VARCHAR(100),
    ExpirationDate DATE NOT NULL
);

CREATE TABLE api_usage (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    UsageDate DATE NOT NULL UNIQUE,
    ItemCount INT DEFAULT 0
);
```

다운로드 후 `tessdata/` 폴더에 넣기

### 6. API 키 발급

**Google Vision API**
```
https://console.cloud.google.com
→ Cloud Vision API 활성화
→ API 키 발급
```

**식품안전처 레시피 DB**
```
https://www.data.go.kr
→ 조리식품의레시피DB 검색
→ 활용신청
→ 인증키 발급
```

---

## ?? 개발 중 겪은 문제 및 해결

### 1. OCR 인식률 문제
**문제:** Tesseract OCR 로 식품 포장지를 인식했으나 인식률이 너무 낮음

**해결:** Google Cloud Vision API 로 교체
```
Tesseract → Google Vision API
인식률 대폭 향상, 한국어/영어 동시 지원
```

### 2. 날짜 형식 다양성 문제
**문제:** 식품마다 날짜 형식이 달라 파싱 실패
```
한국: YY.MM.DD (26.01.12)
외국: DD.MM.YYYY (01.02.2025)
```

**해결:** 외국 식품 모드 추가 + 정규식 패턴 확장
```csharp
string datePattern = @"(\d{2,4})[.\-/](\d{2})[.\-/](\d{2,4})";
```

### 3. 이모지 MySQL 저장 오류
**문제:** 카테고리 이모지 저장 시 `Incorrect string value` 오류

**해결:** MySQL 테이블 인코딩 변경
```sql
ALTER TABLE ingredients
CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
```

### 4. appsettings.json 경로 문제
**문제:** `appsettings.json` 을 `bin/Debug` 폴더에서 못 찾는 오류

**해결:** csproj 파일에 자동 복사 설정 추가
```xml
<Content Include="appsettings.json">
  <CopyToOutputDirectory>Always</CopyToOutputDirectory>
</Content>
```

### 5. 조리순서 번호 중복 문제
**문제:** API 응답에 이미 번호가 있는데 코드에서 번호를 또 붙여서 중복 발생
```
2. 2. 부재채를 잘게...
```

**해결:** 기존 번호 제거 후 재정렬
```csharp
string cleaned = Regex.Replace(manual.Trim(), @"^\d+\.\s*", "");
result += step + ". " + cleaned + "\n\n";
```

### 6. + 버튼 스크롤 문제
**문제:** + 버튼이 스크롤 패널 안에 들어가서 스크롤 시 같이 움직임

**해결:** `Dock = Bottom` 패널에 버튼 배치
```csharp
Anchor = AnchorStyles.Bottom | AnchorStyles.Right
addBtn.BringToFront();
```

### 7. WinForms 구버전 호환 문제
**문제:** `Math.Clamp`, `switch 표현식`, `using var` 등 최신 문법 오류

**해결:** 구버전 호환 문법으로 교체
```csharp
// Math.Clamp → if/else
// switch 표현식 → if/else
// using var → using(){}
```

---

## ?? API 사용량 제한

| API | 무료 한도 | 앱 내 제한 |
|-----|-----------|------------|
| Google Vision | 월 1,000건 | 일 20건 |
| 식품안전처 레시피 | 무제한 | 제한 없음 |

---

## ?? 보안

- API 키는 `appsettings.json` 에 저장
- `.gitignore` 로 Git 업로드 제외
- `appsettings.json` 은 직접 생성 필요

---

## ?? .gitignore

```
# 설정 파일 (API 키 포함)
appsettings.json

# Visual Studio
.vs/
bin/
obj/
*.user
*.suo
packages/

# OCR 언어 파일
tessdata/

# 사용량 기록
ocr_count.txt
```

---

## ????? 개발 환경

```
IDE      : Visual Studio 2019
언어     : C# (.NET Framework)
DB       : MySQL 8.0
OS       : Windows 10/11
```