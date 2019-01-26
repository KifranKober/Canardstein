using IrrlichtLime;
using IrrlichtLime.Core;
using IrrlichtLime.Scene;
using IrrlichtLime.Video;
using System;
using System.Drawing;

namespace Canardstein
{
    public class Jeu
    {
        private static void Main(string[] args) { Jeu jeu = new Jeu(); }

        private IrrlichtDevice Device;
        private uint DerniereFrame = 0;
        private bool K_Avant, K_Arriere, K_Gauche, K_Droite;
        private double Rotation = 0;
        private Vector3Df VecteurAvant = new Vector3Df(1, 0, 0);
        private Vector3Df VecteurDroite = new Vector3Df(0, 0, -1);
        private Texture TextureMur, TextureSol, TexturePlafond, TextureMurDeco;

        public Jeu()
        {
            Device = IrrlichtDevice.CreateDevice(
                DriverType.Direct3D9,
                new Dimension2Di(800, 600),
                32, false, false, true);

            Device.SetWindowCaption("Canardstein 3D");
            Device.OnEvent += Evenement;

            TextureMur = Device.VideoDriver.GetTexture("mur.png");
            TextureSol = Device.VideoDriver.GetTexture("sol.png");
            TexturePlafond = Device.VideoDriver.GetTexture("plafond.png");
            TextureMurDeco = Device.VideoDriver.GetTexture("mur_deco.png");

            /*
            SceneNode cube = Device.SceneManager.AddCubeSceneNode(
                1, null, 0, new Vector3Df(2, 0, 0), new Vector3Df(0, 45, 0));

            cube.SetMaterialFlag(MaterialFlag.Lighting, false);
            cube.SetMaterialTexture(0, TextureMur);
            */
            Bitmap carte = (Bitmap)System.Drawing.Image.FromFile("carte.png");
            for (int x = 0; x < 32; x++)
                for (int y = 0; y < 32; y++)
                {
                    System.Drawing.Color col = carte.GetPixel(x, y);
                    if ((col.R == 255) && (col.G == 255) && (col.B == 255))
                    {
                        SceneNode cube = Device.SceneManager.AddCubeSceneNode(1, null, 0, new Vector3Df(x, 0, y));
                        cube.SetMaterialFlag(MaterialFlag.Lighting, false);
                        cube.SetMaterialTexture(0, TextureMur);
                    }
                    else if ((col.R == 0) && (col.G == 0) && (col.B == 255))
                    {
                        SceneNode cube = Device.SceneManager.AddCubeSceneNode(1, null, 0, new Vector3Df(x, 0, y));
                        cube.SetMaterialFlag(MaterialFlag.Lighting, false);
                        cube.SetMaterialTexture(0, TextureMurDeco);
                    }
                }
            carte.Dispose();

            Mesh meshSol = Device.SceneManager.AddHillPlaneMesh("plan", new Dimension2Df(1, 1), new Dimension2Di(32, 32), null, 0, new Dimension2Df(0, 0), new Dimension2Df(32, 32));

            MeshSceneNode sol = Device.SceneManager.AddMeshSceneNode(meshSol);
            sol.SetMaterialFlag(MaterialFlag.Lighting, false);
            sol.SetMaterialTexture(0, TextureSol);
            sol.Position = new Vector3Df(15.5f, -0.5f, 15.5f);

            MeshSceneNode plafond = Device.SceneManager.AddMeshSceneNode(meshSol);
            plafond.SetMaterialFlag(MaterialFlag.Lighting, false);
            plafond.SetMaterialTexture(0, TexturePlafond);
            plafond.Position = new Vector3Df(15.5f, 0.5f, 15.5f);
            plafond.Rotation = new Vector3Df(180, 0, 0);

            /*
            CameraSceneNode camera = Device.SceneManager.AddCameraSceneNode(
                null, new Vector3Df(0, 0, 0), new Vector3Df(2, 0, 0));
            */
            CameraSceneNode camera = Device.SceneManager.AddCameraSceneNode(null, new Vector3Df(1, 0, 1), new Vector3Df(2, 0, 1));
            camera.NearValue = 0.1f;

            Device.CursorControl.Position = new Vector2Di(400, 300);
            Device.CursorControl.Visible = false;
            
            while (Device.Run())
            {
                float tempsEcoule = (Device.Timer.Time - DerniereFrame) / 1000f;
                DerniereFrame = Device.Timer.Time;

                if(Device.CursorControl.Position.X != 400)
                {
                    Rotation += (Device.CursorControl.Position.X - 400) * 0.0025;
                    Device.CursorControl.Position = new Vector2Di(400, 300);
                    VecteurAvant = new Vector3Df((float)Math.Cos(Rotation), 0, -(float)Math.Sin(Rotation));
                    VecteurDroite = VecteurAvant;
                    VecteurDroite.RotateXZby(-90);
                }

                Vector3Df vitesse = new Vector3Df();
                /*
                if (K_Avant) vitesse.X = 1;
                else if (K_Arriere) vitesse.X = -1;
                if (K_Gauche) vitesse.Z = 1;
                else if (K_Droite) vitesse.Z = -1;
                */
                if (K_Avant) vitesse += VecteurAvant;
                else if (K_Arriere) vitesse -= VecteurAvant;
                if (K_Gauche) vitesse -= VecteurDroite;
                else if (K_Droite) vitesse += VecteurDroite;

                vitesse = vitesse.Normalize() * tempsEcoule * 2;
                camera.Position += vitesse;
                //camera.Target = camera.Position + new Vector3Df(1, 0, 0);
                camera.Target = camera.Position + VecteurAvant;

                // Device.VideoDriver.BeginScene(ClearBufferFlag.Color | ClearBufferFlag.Depth, Color.OpaqueMagenta);
                Device.VideoDriver.BeginScene(ClearBufferFlag.Color | ClearBufferFlag.Depth, IrrlichtLime.Video.Color.OpaqueMagenta);

                Device.SceneManager.DrawAll();

                Device.VideoDriver.EndScene();
            }
        }

        private bool Evenement(Event e)
        {
            if(e.Type == EventType.Key)
            {
                switch (e.Key.Key)
                {
                    case KeyCode.KeyZ: K_Avant = e.Key.PressedDown; break;
                    case KeyCode.KeyS: K_Arriere = e.Key.PressedDown; break;
                    case KeyCode.KeyQ: K_Gauche = e.Key.PressedDown; break;
                    case KeyCode.KeyD: K_Droite = e.Key.PressedDown; break;
                }
            }
            return false;
        }    
    }
}

