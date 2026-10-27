#!/bin/bash

# ==============================================================================
#  dev.sh — Sobe o Sistema Serralheria-Paredes (.NET 10) + Cloudflare Quick Tunnel
#  Gera URL pública temporária HTTPS com SSL automático gerenciado pelo Cloudflare
#  Uso: ./dev.sh
#  Para encerrar: Ctrl+C
# ==============================================================================

PROJECT_DIR="$(cd "$(dirname "$0")" && pwd)"
PORT=5000
LOG_CF="/tmp/cf_tunnel_serralheria_$$.log"

# Cores do Terminal
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
RED='\033[0;31m'
NC='\033[0m'

cleanup() {
    echo -e "\n${RED}⏹  Encerrando Serralheria-Paredes e Túnel Cloudflare...${NC}"
    kill "$DOTNET_PID" "$CF_PID" "$MONITOR_PID" "$TAIL_PID" 2>/dev/null
    wait "$DOTNET_PID" "$CF_PID" "$MONITOR_PID" "$TAIL_PID" 2>/dev/null
    rm -f "$LOG_CF"
    echo -e "${GREEN}✔  Processos finalizados com sucesso.${NC}"
    exit 0
}
trap cleanup INT TERM

echo -e "${CYAN}🚀 Iniciando Serralheria-Paredes (.NET 10) na porta ${PORT}...${NC}"
cd "$PROJECT_DIR" || { echo -e "${RED}Pasta não encontrada: $PROJECT_DIR${NC}"; exit 1; }

dotnet run --launch-profile http &
DOTNET_PID=$!

# Aguarda a aplicação subir
echo -e "${YELLOW}⏳ Aguardando aplicação ficar pronta...${NC}"
for i in $(seq 1 45); do
    STATUS=$(curl -s -o /dev/null -w "%{http_code}" "http://127.0.0.1:${PORT}" 2>/dev/null)
    if [[ "$STATUS" =~ ^[1-9][0-9][0-9]$ ]]; then
        break
    fi
    if ! kill -0 "$DOTNET_PID" 2>/dev/null; then
        echo -e "${RED}❌ A aplicação encerrou inesperadamente.${NC}"
        exit 1
    fi
    sleep 1
done

echo -e "${GREEN}✔  Serralheria-Paredes rodando localmente em http://127.0.0.1:${PORT}${NC}"
echo -e "${CYAN}🌐 Conectando Cloudflare Quick Tunnel (HTTPS temporário)...${NC}"

touch "$LOG_CF"

# --config /dev/null → ignora configurações locais do ~/.cloudflared/config.yml
cloudflared tunnel --config /dev/null --no-autoupdate --url "http://127.0.0.1:${PORT}" > "$LOG_CF" 2>&1 &
CF_PID=$!

# Monitora o log em busca da URL pública HTTPS
(
    URL_SHOWN=false
    while kill -0 "$CF_PID" 2>/dev/null; do
        if [ "$URL_SHOWN" = false ]; then
            URL=$(grep -oE "https://[a-zA-Z0-9._-]+\.trycloudflare\.com" "$LOG_CF" 2>/dev/null | head -1)
            if [ -n "$URL" ]; then
                echo -e "\n${GREEN}╔══════════════════════════════════════════════════════════════════════╗${NC}"
                echo -e "${GREEN}║  ✅ URL PÚBLICA HTTPS TEMPORÁRIA (Serralheria-Paredes):              ║${NC}"
                echo -e "${GREEN}║  ${YELLOW}${URL}${GREEN}                       ║${NC}"
                echo -e "${GREEN}╚══════════════════════════════════════════════════════════════════════╝${NC}"
                echo -e "${CYAN}   Pressione Ctrl+C para encerrar a aplicação e o túnel.${NC}\n"
                URL_SHOWN=true
            fi
        fi
        sleep 1
    done
) &
MONITOR_PID=$!

# Exibe o log ao vivo do cloudflared
tail -f "$LOG_CF" &
TAIL_PID=$!

wait "$DOTNET_PID"
kill "$CF_PID" "$MONITOR_PID" "$TAIL_PID" 2>/dev/null
