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

while [ $(curl -o -I -L -s -w "%{http_code}" http://mmq-service:9000/healthcheck) -ne 200 ]
do
    echo -n "kerstel"
    sleep 1
done

while [ $(curl -o -I -L -s -w "%{http_code}" http://mmq-service-hapi:1000/status) -ne 200 ]
do
    echo -n "hapi"
    sleep 1
done

EOF

# https://stackoverflow.com/questions/9869902/prevent-bash-from-interpreting-without-quoting-everything
cat >> /app/wrk/http-ready-mmq.sh << 'EOF'
#!/bin/bash
while [ $(curl -o -I -L -s -w "%{http_code}" http://mmq-service:9000/healthcheck) -ne 200 ]
do
    echo -n "kerstel"
    sleep 1
done

EOF

chmod +x /app/wrk/http-ready-mmq.sh

echo 'Creating status.sh'
cat >> /app/wrk/status.sh << EOF
#!/bin/bash
echo ''
echo '- Make sure to check CPU and RAM saturation.'
echo ''
./http-ready.sh
./wrk -t12 -c400 -d10s http://mmq-service-nodejs:8000/status
./wrk -t12 -c400 -d10s http://mmq-service-express:4000/status
./wrk -t12 -c400 -d10s http://mmq-service-hapi:1000/status
./wrk -t12 -c400 -d10s http://mmq-service:9000/status
./wrk -t12 -c400 -d10s http://mmq-service:9000/healthcheck
EOF

chmod +x /app/wrk/status.sh


echo 'Creating misc.sh'
cat >> /app/wrk/misc.sh << EOF
#!/bin/bash
set -e
echo ''
echo '- Make sure to check CPU and RAM saturation.'
echo ''
./http-ready.sh
echo ''
echo '** EF CORE IN-MEMORY DB **'
./wrk -t1 -c5 -d5s -s ./scripts/mmq-post.lua http://mmq-service:9000/efcore-in-mem-text
echo ''
echo '** XML 15C **'
./wrk -t2 -c15 -d7s -s ./scripts/mmq-post-xml.lua http://mmq-service:9000/send
echo ''
echo '** LARGE JSON 15C **'
./wrk -t2 -c15 -d7s -s ./scripts/mmq-post-json-2.lua http://mmq-service:9000/send
echo ''
echo '** JSON 400C **'
./wrk -t12 -c400 -d7s -s ./scripts/mmq-post.lua http://mmq-service:9000/send
EOF

chmod +x /app/wrk/misc.sh

echo 'Creating post_message.sh'
cat >> /app/wrk/post_message.sh << EOF
#!/bin/bash
set -e
echo ''
echo '- Make sure to check CPU and RAM saturation.'
echo ''
./http-ready-mmq.sh
echo ''
echo '** EF CORE IN-MEMORY DB **'
./wrk -t1 -c5 -d5s -s ./scripts/mmq-post.lua http://mmq-service:9000/efcore-in-mem-text
echo ''
echo '** JSON 400C **'
./wrk -t12 -c400 -d12s -s ./scripts/mmq-post.lua http://mmq-service:9000/send
EOF

chmod +x /app/wrk/post_message.sh

echo 'Creating arm.sh'
cat >> /app/wrk/arm.sh << EOF
#!/bin/bash
set -v
echo ''
echo '- Make sure to check CPU and RAM saturation.'
echo ''
./http-ready.sh
./wrk -t1 -c5 -d7s -s ./scripts/mmq-post.lua http://mmq-service:9000/send
EOF

chmod +x /app/wrk/arm.sh

echo 'Creating arm1.sh'
cat >> /app/wrk/arm1.sh << EOF
#!/bin/bash
set -v
echo ''
echo '- Make sure to check CPU and RAM saturation.'
echo ''
./http-ready.sh
./wrk -t1 -c5 -d5s -s ./scripts/mmq-post.lua http://mmq-service:9000/efcore-in-mem-text
./wrk -t4 -c40 -d3s -s ./scripts/mmq-post.lua http://mmq-service:9000/faster-get
EOF

chmod +x /app/wrk/arm1.sh


touch /opt/install_done
echo 'Done!'
