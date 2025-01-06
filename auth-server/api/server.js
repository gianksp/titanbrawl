const express = require("express");
const cors = require("cors");
const axios = require("axios");

const app = express();

// Environment Variables
const CLIENT_ID = process.env.CLIENT_ID;
const CLIENT_SECRET = process.env.CLIENT_SECRET;
const REDIRECT_URI = "https://titanbrawl.vercel.app/callback";

// In-memory session storage (not recommended for production)
const sessions = {};

app.use(cors());
app.use(express.json());
app.use(express.urlencoded({ extended: true }));

// Root endpoint
app.get("/", (req, res) => {
  const url = `https://github.com/login/oauth/authorize?client_id=${CLIENT_ID}&redirect_uri=${REDIRECT_URI}&scope=read:user`;
  res.send(`<a href="${url}">Login with GitHub</a>`);
});

// GitHub OAuth callback
app.get("/callback", async (req, res) => {
  const code = req.query.code;
  const state = req.query.state;

  if (!code || !state) {
    return res.status(400).send("Missing code or state parameter");
  }

  try {
    const tokenResponse = await axios.post(
      "https://github.com/login/oauth/access_token",
      {
        client_id: CLIENT_ID,
        client_secret: CLIENT_SECRET,
        code,
      },
      { headers: { Accept: "application/json" } }
    );

    const accessToken = tokenResponse.data.access_token;

    const userResponse = await axios.get("https://api.github.com/user", {
      headers: { Authorization: `Bearer ${accessToken}` },
    });

    const user = userResponse.data;
    sessions[state] = { user, accessToken };

    res.send("Authentication successful. Return to Unity.");
  } catch (error) {
    console.error("OAuth error:", error);
    res.status(500).send("Authentication failed");
  }
});

app.post("/logout", (req, res) => {
  const sessionId = req.body.sessionId;

  if (!sessionId || !sessions[sessionId]) {
    return res.status(404).json({ error: "Invalid session ID" });
  }

  delete sessions[sessionId];
  res.status(200).json({ message: "Logout successful" });
});

app.get("/getUserInfo", (req, res) => {
  const sessionId = req.query.sessionId;

  if (!sessionId || !sessions[sessionId]) {
    return res.status(404).json({ error: "Session not found" });
  }

  res.json(sessions[sessionId]);
});

module.exports = app;
