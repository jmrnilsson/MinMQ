#!/bin/bash
set -e
set -u

echo "Run once"
if [ -f /opt/install_done ]; then
	echo "Error. Installation already completed once."
	exit 0
fi

# https://stackoverflow.com/questions/9869902/prevent-bash-from-interpreting-without-quoting-everything
cat >> /app/wrk/http-ready.sh << 'EOF'
#!/bin/bash
while [ $(curl -o -I -L -s -w "%{http_code}" http://mmq-service-express:4000/status) -ne 200 ]
do
    echo -n "express"
    sleep 1
done

while [ $(curl -o -I -L -s -w "%{http_code}" http://mmq-service-nodejs:8000/status) -ne 200 ]
do
    echo -n "node"
    sleep 1
done

while [ $(curl -o -I -L -s -w "%{http_code}" http://mmq-service-kestrel:9000/status) -ne 200 ]
do
    echo -n "kerstel"
    sleep 1
done

EOF

chmod +x /app/wrk/http-ready.sh

echo 'Creating status.sh'
cat >> /app/wrk/status.sh << EOF
#!/bin/bash
echo ''
echo '- Make sure to check CPU and RAM saturation.'
echo ''
./http-ready.sh
echo ''; ./wrk -t12 -c400 -d30s http://mmq-service-nodejs:8000/status
echo ''; ./wrk -t12 -c400 -d30s http://mmq-service-express:4000/status
echo ''; ./wrk -t12 -c400 -d30s http://mmq-service-kestrel:9000/status
echo ''; ./wrk -t12 -c400 -d30s http://mmq-service-kestrel:9000/status-async
EOF

chmod +x /app/wrk/status.sh

touch /opt/install_done
echo 'Done!'