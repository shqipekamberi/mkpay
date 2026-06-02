#!/bin/bash
echo "Testing MKPay API..."
echo "======================"

API_URL="http://localhost:5251"
echo "API URL: $API_URL"

# Test 1: Health endpoint
echo ""
echo "1. Testing health endpoint..."
response=$(curl -s -w "%{http_code}" "$API_URL/api/health")
http_code=${response: -3}
body=${response%???}

if [ "$http_code" -eq 200 ]; then
    echo "✅ Health endpoint OK (HTTP $http_code)"
    echo "Response: $body"
else
    echo "❌ Health endpoint FAILED (HTTP $http_code)"
    echo "Response: $body"
fi

# Test 2: Swagger endpoint
echo ""
echo "2. Testing Swagger UI..."
swagger_code=$(curl -s -o /dev/null -w "%{http_code}" "$API_URL/swagger/index.html")
if [ "$swagger_code" -eq 200 ]; then
    echo "✅ Swagger UI available (HTTP $swagger_code)"
else
    echo "⚠️ Swagger UI not available (HTTP $swagger_code)"
fi

# Test 3: Registration endpoint (without auth)
echo ""
echo "3. Testing user registration..."
RANDOM_EMAIL="test_$(date +%s)@mkpay.com"
RANDOM_USER="user_$(date +%s)"

registration_response=$(curl -s -w "%{http_code}" -X POST "$API_URL/api/Auth/register" \
  -H "Content-Type: application/json" \
  -d "{
    \"email\": \"$RANDOM_EMAIL\",
    \"username\": \"$RANDOM_USER\",
    \"password\": \"Test123!\",
    \"firstName\": \"Test\",
    \"lastName\": \"User\",
    \"phoneNumber\": \"38970123456\"
  }")

reg_http_code=${registration_response: -3}
reg_body=${registration_response%???}

if [ "$reg_http_code" -eq 200 ]; then
    echo "✅ Registration successful (HTTP $reg_http_code)"
    # Extract token if available
    token=$(echo "$reg_body" | grep -o '"token":"[^"]*"' | cut -d'"' -f4)
    if [ -n "$token" ]; then
        echo "   Token received: ${token:0:20}..."
    fi
else
    echo "❌ Registration failed (HTTP $reg_http_code)"
    echo "   Response: $reg_body"
fi

# Test 4: Login with the registered user
echo ""
echo "4. Testing login..."
if [ -n "$RANDOM_EMAIL" ]; then
    login_response=$(curl -s -w "%{http_code}" -X POST "$API_URL/api/Auth/login" \
      -H "Content-Type: application/json" \
      -d "{
        \"email\": \"$RANDOM_EMAIL\",
        \"password\": \"Test123!\"
      }")
    
    login_http_code=${login_response: -3}
    login_body=${login_response%???}
    
    if [ "$login_http_code" -eq 200 ]; then
        echo "✅ Login successful (HTTP $login_http_code)"
        # Extract token for next test
        token=$(echo "$login_body" | grep -o '"token":"[^"]*"' | cut -d'"' -f4)
        if [ -n "$token" ]; then
            echo "   Token: ${token:0:20}..."
            TEST_TOKEN="$token"
        fi
    else
        echo "❌ Login failed (HTTP $login_http_code)"
        echo "   Response: $login_body"
    fi
else
    echo "⚠️ Skipping login test (registration failed)"
fi

# Test 5: Authenticated endpoint (if we have a token)
echo ""
echo "5. Testing authenticated endpoint..."
if [ -n "$TEST_TOKEN" ]; then
    auth_response=$(curl -s -w "%{http_code}" "$API_URL/api/Account/me" \
      -H "Authorization: Bearer $TEST_TOKEN")
    
    auth_http_code=${auth_response: -3}
    auth_body=${auth_response%???}
    
    if [ "$auth_http_code" -eq 200 ]; then
        echo "✅ Authenticated endpoint OK (HTTP $auth_http_code)"
        # Extract account number
        acc_number=$(echo "$auth_body" | grep -o '"accountNumber":"[^"]*"' | cut -d'"' -f4)
        if [ -n "$acc_number" ]; then
            echo "   Account Number: $acc_number"
        fi
    else
        echo "❌ Authentication failed (HTTP $auth_http_code)"
        echo "   Response: $auth_body"
    fi
else
    echo "⚠️ Skipping auth test (no token available)"
fi

echo ""
echo "======================"
echo "Test completed!"
