#!/bin/bash
# ============================================================
# Docker DB initialisation — runs once on first container start
# Mounted to /docker-entrypoint-initdb.d/00_init.sh
# SQL scripts are mounted at /db/
# ============================================================
set -e

echo "==> Running JobPortal database setup..."

# cd into /db so that \i relative paths in run_all.sql resolve correctly
cd /db && psql -v ON_ERROR_STOP=1 \
               --username "$POSTGRES_USER" \
               --dbname   "$POSTGRES_DB" \
               --file run_all.sql

echo "==> Database setup complete."
