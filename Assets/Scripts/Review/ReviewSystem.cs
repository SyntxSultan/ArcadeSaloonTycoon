using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public enum LikedEnum { Like, Dislike }

[Serializable]
public class Review
{
    public int id;
    public string header;
    public string description;
    public int stars; // 1..5
    public LikedEnum liked;
    public DateTime timestamp;

    public Review(int id, string header, string description, int stars, LikedEnum liked)
    {
        this.id = id;
        this.header = header;
        this.description = description;
        this.stars = Mathf.Clamp(stars, 1, 5);
        this.liked = liked;
        this.timestamp = DateTime.UtcNow; // stored but UI won't show it per request
    }
}

public class ReviewSystem : MonoBehaviour
{
    // Singleton so you can call ReviewSystem.MakeReview(...) from anywhere.
    public static ReviewSystem Instance { get; private set; }

    private List<Review> reviews = new List<Review>();
    private int nextId = 1;

    // Events - UI subscribes to these
    public event Action<Review> OnReviewAdded;
    public event Action OnReviewsChanged;

    private void Awake()
    {
        Instance = this;
    }

    // Static convenience method the user requested
    public static void MakeReview(string header, string description, int stars, LikedEnum liked)
    {
        if (Instance == null)
        {
            Debug.LogError("ReviewSystem instance not found in scene. Add ReviewSystem to a GameObject.");
            return;
        }
        Instance.AddReviewInternal(header, description, stars, liked);
    }

    private void AddReviewInternal(string header, string description, int stars, LikedEnum liked)
    {
        var r = new Review(nextId++, header, description, stars, liked);
        reviews.Add(r);
        OnReviewAdded?.Invoke(r);
        OnReviewsChanged?.Invoke();
    }

    // Read-only accessors for UI & other systems
    public IReadOnlyList<Review> GetAllReviews() => reviews.AsReadOnly();

    public int GetTotalReviews() => reviews.Count;

    public float GetAverageStars()
    {
        if (reviews.Count == 0) return 0.0f;
        float sum = 0;
        foreach (var r in reviews) sum += r.stars;
        return sum / reviews.Count;
    }

    // Optional: clear reviews (useful in editor / testing)
    [Button("Clear All Reviews")]
    protected void ClearAllReviews()
    {
        reviews.Clear();
        nextId = 1;
        OnReviewsChanged?.Invoke();
    }
}
