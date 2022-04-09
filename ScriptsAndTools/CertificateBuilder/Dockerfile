FROM debian:bullseye

RUN apt-get update
RUN apt-get install -y openssl
ENV CA_NAME "ca"
ENV SERVER_NAME "server"
ENV CA_DURATION "36500"
ENV SERVER_DURATION "3650"
WORKDIR /script
COPY buildCert.sh /script/
RUN chmod +x /script/buildCert.sh
WORKDIR /work

ENTRYPOINT [ "/script/buildCert.sh" ]