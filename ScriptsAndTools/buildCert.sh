#!/bin/bash
CA_NAME="${1:-ca}"
CA_DURATION="${2:-36500}"
SERVER_NAME="${3:-server}"
SERVER_DURATION="${4:-3650}"
if [ ! -f "Certificates/$CA_NAME.crt" ]; then
    echo "Certificates/$CA_NAME.crt does not exist, creating it..."
    openssl genrsa -des3 -out "Certificates/$CA_NAME.key" 2048
    openssl req -new -x509 -days $CA_DURATION -key "Certificates/$CA_NAME.key" -out "Certificates/$CA_NAME.crt"
fi
openssl genrsa -out "Certificates/$SERVER_NAME.key" 2048
openssl req -new -out "Certificates/$SERVER_NAME.csr" -key "Certificates/$SERVER_NAME.key"
openssl x509 -req -in "Certificates/$SERVER_NAME.csr" -CA "Certificates/$CA_NAME.crt" -CAkey "Certificates/$CA_NAME.key" -CAcreateserial -out "Certificates/$SERVER_NAME.crt" -days $SERVER_DURATION