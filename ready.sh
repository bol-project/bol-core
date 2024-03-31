#!/bin/sh

# Extract the ordinal number from the hostname
ordinal=$(echo $HOSTNAME | rev | cut -d'-' -f1 | rev)

# For pod-0, there is no previous pod
if [ "$ordinal" -eq 0 ]; then
    echo "This is the first pod, so it's always ready."
    exit 0
fi

# Construct the hostname of the previous pod
previous_ordinal=$((ordinal - 1))
statefulset_name=$(echo $HOSTNAME | rev | cut -d'-' -f2- | rev)
previous_pod_hostname="$statefulset_name-$previous_ordinal.hl-$statefulset_name"
echo $previous_pod_hostname
 
# Prepare the JSON-RPC request payload
request_body='{
  "jsonrpc": "2.0",
  "id": 1,
  "method": "getVersion",
  "params": []
}'

# Make the HTTP POST request to the previous pod
response=$(curl -s -o /dev/null -w "%{http_code}" -X POST http://${previous_pod_hostname}:21332 -d "$request_body" -H "Content-Type: application/json")

# Check the response
if [ "$response" -eq 200 ]; then
    echo "Previous pod is ready, so this pod should not be ready."
    exit 1
else
    echo "Previous pod is not ready, so this pod should be ready."
    exit 0
fi