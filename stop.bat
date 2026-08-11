@echo off
echo Stopping PostgreSQL container...
docker compose down
echo.
echo Done. Close the backend/frontend terminal windows manually to stop those.
pause
