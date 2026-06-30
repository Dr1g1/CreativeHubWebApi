import { BrowserRouter, Routes, Route } from "react-router-dom";
import { AuthProvider } from "./context/AuthContext";
import Navbar from "./components/Navbar";
import HomePage from "./pages/HomePage";
import LoginPage from "./pages/LoginPage";
import RegisterPage from "./pages/RegisterPage";
import ResourceDetailPage from "./pages/ResourceDetailPage";
import UploadPage from "./pages/UploadPage";
import ProtectedRoute from "./components/ProtectedRoute";
import ProfilePage from "./pages/ProfilePage";
import CollectionsPage from "./pages/CollectionsPage";
import CollectionDetailPage from "./pages/CollectionDetailPage";
import StatisticsPage from "./pages/StatisticsPage";

function App() {
  return (
    <AuthProvider>
      <BrowserRouter>
        <Navbar />
        <Routes>
          <Route path="/" element={<HomePage />} />
          <Route path="/login" element={<LoginPage />} />
          <Route path="/register" element={<RegisterPage />} />
          <Route path="/resources/:id" element={<ResourceDetailPage />} />
          <Route path="/upload" 
                element={ 
                  <ProtectedRoute> 
                    <UploadPage />
                  </ProtectedRoute>
                }
          />
          <Route
            path="/profile"
            element={
              <ProtectedRoute>
                <ProfilePage />
              </ProtectedRoute>
            }
          />
          <Route
            path="/collections"
            element={
              <ProtectedRoute>
                <CollectionsPage />
              </ProtectedRoute>
            }
          />
          <Route path="/collections/:id" element={<CollectionDetailPage />} />
          <Route path="/statistics" element={<StatisticsPage />} />
        </Routes>
      </BrowserRouter>
    </AuthProvider>
  );
}

export default App;