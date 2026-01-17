FROM node:20-slim

# Install build dependencies for serialport (python3, make, g++, etc.)
# typically serialport needs these to compile native bindings
RUN apt-get update && apt-get install -y python3 make g++ udev && rm -rf /var/lib/apt/lists/*

WORKDIR /app

COPY package*.json ./
RUN npm install

COPY . .

# Compile TS to JS if needed, or run via ts-node
# For production, it's efficient to build, but ts-node is fine for this scale
# CMD ["npm", "start"] works if package.json has "start": "ts-node src/server.ts"

EXPOSE 3000

CMD ["npm", "start"]
