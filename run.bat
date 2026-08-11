@echo off
setlocal

echo ============================================
echo  Analytics Dashboard - one-click startup
echo ============================================
echo.
echo Make sure Docker Desktop is open and running before continuing.
pause

echo.
echo [1/4] Starting PostgreSQL (Docker)...
docker compose up -d
if errorlevel 1 (
    echo.
    echo Could not start Docker. Make sure Docker Desktop is open ^(green/running^), then run this script again.
    pause
    exit /b 1
)

echo.
echo [2/4] Waiting for PostgreSQL to become healthy...
set RETRIES=0
:waitloop
for /f "usebackq delims=" %%H in (`docker inspect -f "{{.State.Health.Status}}" analytics-dashboard-db 2^>nul`) do set HEALTH=%%H
if "%HEALTH%"=="healthy" goto ready
set /a RETRIES+=1
if %RETRIES% GEQ 40 (
    echo.
    echo PostgreSQL did not become healthy in time. Run this script again ^(the volume is already created, it should be faster next time^).
    pause
    exit /b 1
)
timeout /t 2 /nobreak >nul
goto waitloop
:ready
echo PostgreSQL is ready.

echo.
echo [3/4] Preparing backend...
cd backend\AnalyticsDashboard.Api
dotnet tool install --global dotnet-ef >nul 2>&1
if not exist Migrations (
    echo Creating initial database migration ^(first run only^)...
    dotnet ef migrations add InitialCreate
)
start "Analytics Dashboard - Backend (http://localhost:5080)" cmd /k "dotnet run"
cd ..\..

echo.
echo [4/4] Preparing frontend...
cd frontend
if not exist node_modules (
    echo Installing frontend packages, this may take a minute ^(first run only^)...
    call npm install
)
if not exist node_modules\.bin\vite.cmd (
    echo Frontend packages look incomplete, reinstalling...
    rmdir /s /q node_modules >nul 2>&1
    del /f /q package-lock.json >nul 2>&1
    call npm install
)
start "Analytics Dashboard - Frontend (http://localhost:5173)" cmd /k "npm run dev"
cd ..

echo.
echo ============================================
echo  All set! Two new windows just opened.
echo  Backend:  http://localhost:5080/swagger
echo  Frontend: http://localhost:5173
echo.
echo  Login with: admin / Admin@123
echo ============================================
echo.
pause
