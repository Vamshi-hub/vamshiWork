# FROM johnpapa/angular-cli as angular-built
# Using the above image allows us toskip the angular-cli install
# FROM node:8.9-alpine as angular-built
# WORKDIR /usr/src/app
# RUN npm i -g @angular/cli
# COPY package.json package.json
# RUN npm install --silent
# COPY . .
# RUN ng build --prod --build-optimizer

FROM nginx:alpine
## Copy our default nginx config
COPY nginx/default.conf /etc/nginx/conf.d/
## Remove default nginx website
RUN rm -rf /usr/share/nginx/html/*    
## From 'builder' stage copy over the artifacts in dist folder to default nginx public folder
## COPY --from=angular-built /usr/src/app/dist /usr/share/nginx/html
# Build in localhost then copy dist folder to docker image
# ng build --prod --build-optimizer
COPY ./dist /usr/share/nginx/html
EXPOSE 80 443
CMD [ "nginx", "-g", "daemon off;" ]