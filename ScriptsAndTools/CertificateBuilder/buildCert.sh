#!/bin/bash
if [ ! -f "/work/$CA_NAME.crt" ]; then
    echo "/work/$CA_NAME.crt does not exist, creating it..."
    openssl genrsa -des3 -out "/work/$CA_NAME.key" 2048
    openssl req -new -x509 -days $CA_DURATION -key "/work/$CA_NAME.key" -out "/work/$CA_NAME.crt"
fi
openssl genrsa -out "/work/$SERVER_NAME.key" 2048
openssl req -new -out "/work/$SERVER_NAME.csr" -key "/work/$SERVER_NAME.key"
openssl x509 -req -in "/work/$SERVER_NAME.csr" -CA "/work/$CA_NAME.crt" -CAkey "/work/$CA_NAME.key" -CAcreateserial -out "/work/$SERVER_NAME.crt" -days $SERVER_DURATION