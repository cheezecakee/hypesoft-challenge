// next.config.mjs
import dotenv from "dotenv";
dotenv.config({ path: ".env.example" });

/** @type {import('next').NextConfig} */
const nextConfig = {
  reactStrictMode: false,
};

export default nextConfig;
