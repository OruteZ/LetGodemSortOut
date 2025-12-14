// using UnityEngine;
//
// namespace BeatTemplate 
// {
//     [CreateAssetMenu(menuName = "BeatTemplate/Track", fileName = "NewTrack")]
//     public class Track : ScriptableObject
//     {
//         [SerializeField] private AudioClip music;
//         [SerializeField] private double bpm;
//         [SerializeField] private double offset;
//         
//         [Header("MetaData")]
//         [SerializeField] private string trackName;
//         [SerializeField] private Sprite coverImage;
//         [SerializeField] private string artistName;
//         [SerializeField] private float previewTime;
//         
//         public AudioClip Music => music;
//         public double Bpm => bpm;
//         public double Bps => bpm / 60.0;
//         public double Spb => 60.0 / bpm;
//         public double Offset => offset;
//         
//         public string TrackName => trackName;
//         public Sprite CoverImage => coverImage;
//         public string ArtistName => artistName;
//         public float PreviewTime => previewTime;
//     }   
// }
//
