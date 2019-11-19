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
# ./wrk -t12 -c400 -d10s -s ./post.lua http://mmq-service-kestrel:9000/message-text
# ./wrk -t12 -c400 -d10s -s ./post-large.lua http://mmq-service-kestrel:9000/message-text
# ./wrk -t12 -c400 -d10s -s ./post-json.lua http://mmq-service-kestrel:9000/message
# ./wrk -t12 -c400 -d10s -s ./post-large.lua http://mmq-service-kestrel:9000/message-sync
# ./wrk -t12 -c400 -d10s http://mmq-service-nodejs:8000/status
# ./wrk -t12 -c400 -d10s http://mmq-service-express:4000/status
# ./wrk -t12 -c400 -d10s http://mmq-service-kestrel:9000/status
# ./wrk -t12 -c400 -d10s -s ./post.lua http://mmq-service-kestrel:9000/message-sync
./wrk -t12 -c400 -d10s -s ./post.lua http://mmq-service-kestrel:9000/message
./wrk -t12 -c400 -d10s -s ./post.lua http://mmq-service-kestrel:9000/faster
EOF

chmod +x /app/wrk/status.sh


echo 'Creating post_message.sh'
cat >> /app/wrk/post_message.sh << EOF
#!/bin/bash
echo ''
echo '- Make sure to check CPU and RAM saturation.'
echo ''
./http-ready.sh
./wrk -t1 -c5 -d5s -s ./post.lua http://mmq-service-kestrel:9000/message-text
# ./wrk -t2 -c13 -d5s -s ./post.lua http://mmq-service-kestrel:9000/faster-text
./wrk -t1 -c5 -d7s -s ./post.lua http://mmq-service-kestrel:9000/faster
./wrk -t2 -c15 -d7s -s ./post.lua http://mmq-service-kestrel:9000/faster
./wrk -t12 -c400 -d7s -s ./post.lua http://mmq-service-kestrel:9000/faster
EOF

chmod +x /app/wrk/post_message.sh


touch /opt/install_done
echo 'Done!'