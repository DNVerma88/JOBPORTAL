FROM postgres:17-alpine

# Copy all SQL scripts into the image
COPY . /db/

# Create the init script that runs from /db so \i relative paths resolve
RUN printf '#!/bin/sh\nset -e\necho "==> Running JobPortal DB setup..."\ncd /db && psql -v ON_ERROR_STOP=1 -U "$POSTGRES_USER" -d "$POSTGRES_DB" -f run_all.sql\necho "==> DB setup complete."\n' \
    > /docker-entrypoint-initdb.d/00_init.sh \
 && chmod +x /docker-entrypoint-initdb.d/00_init.sh
