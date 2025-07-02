# AIPS Traffic Data Analysis Console App

This is a console application that processes traffic data collected by an automated half-hourly car counter. It reads timestamped traffic counts from a text file and outputs analytical reports to the console.

---

## Features

-  Reads timestamped half-hourly car count data from a file (currently txt)
-  Calculates the **total number of cars seen**
-  Displays **daily car counts**
-  Shows the **top 3 half-hour periods with the most cars**
-  Identifies the **1.5-hour period (3 half-hours) with the fewest cars**, ensuring timestamps are exactly 30 minutes apart

---

## 🗂 Example Input File

The input file should be named `data.txt` and placed in the working directory. It must contain lines formatted like:

```
2021-12-01T05:00:00 5
2021-12-01T05:30:00 12
2021-12-01T06:00:00 14
```

Each line represents a half-hour period and the number of cars observed.

---

## ▶️ How to Run

### 🛠 Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download) or newer (8 is chosen as it is a lts release)

### 🧪 (Optional) Run Unit Tests

If you have a test project:

```bash
dotnet test
```

### 🚀 Run the Console App

From the root of the solution (or wherever the `.csproj` is located):

```bash
dotnet run --project AIPS.ConsoleApp
```

Make sure the file `data.txt` is present in the same directory as `Program.cs` or provide the correct path in the code.

---

## ✅ Sample Output

```
Total Cars: 322

Cars Per Day:
2021-12-01 179
2021-12-05 81
2021-12-08 134

Top 3 Half Hours:
2021-12-01T07:30:00 46
2021-12-01T08:00:00 42
2021-12-01T07:00:00 25

The 1.5 hour period with least cars:
2021-12-01T15:00:00 9
2021-12-01T15:30:00 11
2021-12-01T23:30:00 0
```

---

## 📌 Notes

- The input is assumed to be clean and well-formatted.
- All timestamps are parsed using `DateTimeOffset`.
- The logic skips non-contiguous 30-minute intervals when searching for the lowest-traffic period.

- Small tangent but the City of Melbourne has datasets for similar scenarios publicly available [https://data.melbourne.vic.gov.au/explore/?sort=modified&q=Sensors]
---

