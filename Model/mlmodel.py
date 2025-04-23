from matplotlib.colors import ListedColormap
import pandas as pd
import numpy as np
from scipy import stats
from sklearn.cluster import KMeans
from sklearn.preprocessing import StandardScaler, MinMaxScaler
from sklearn.metrics import silhouette_score, davies_bouldin_score, calinski_harabasz_score
from sklearn.decomposition import PCA
import matplotlib.pyplot as plt
import plotly.express as px

"""
blue - both players cheating
green - client cheating
yellow - host cheating
purple - no cheating
"""



pd.set_option('display.max_rows', None)
pd.set_option('display.max_columns', None)
pd.set_option('display.max_colwidth', None)

colors = ['red', 'blue', 'yellow', 'orange']

# Load data (columns: illegal_traces, trigger_time, flick_speed)
data = pd.read_csv('datapoints/combined_data.csv')

# Feature Engineering: Convert Trigger Time to Trigger Speed (higher = cheating)
epsilon = 0.00001  # Prevent division by zero
data['trigger_time'] = data['trigger_time'].replace(0, 10)
data['trigger_time'] = 1 / (data['trigger_time'])

threshold_z = 2
trigger_z = np.abs(stats.zscore(data['trigger_time']))
flick_z = np.abs(stats.zscore(data['flick_speed']))
trace_z = np.abs(stats.zscore(data['illegal_traces']))

trigger_outlier = np.where(trigger_z > threshold_z)[0]
flick_outlier = np.where(flick_z > threshold_z)[0]
trace_z = np.where(trace_z > threshold_z)[0]

data['trigger_time'] = data['trigger_time'].drop(trigger_outlier)
data['flick_speed'] = data['flick_speed'].drop(flick_outlier)
data['illegal_traces'] = data['illegal_traces'].drop(trace_z)

data.dropna(how='any', inplace=True)


# create a new composite feature that combines all of the 3 statistics to force joint effect
data["cheating_score"] = (((data['illegal_traces'] / 50)  + (data['flick_speed'] / 100 )) + (data['trigger_time']) * 100 )

print(data)

X = data['cheating_score'].values.reshape(-1,1)

# Select and standardize features
scaler = StandardScaler()
X_scaled = scaler.fit_transform(X)

# Cluster data into 4 groups using K-Means
kmeans = KMeans(n_clusters=4, random_state=42)
kmeans.fit(X_scaled)

centroids = kmeans.cluster_centers_.flatten()
sorted_clusters = np.argsort(centroids)[::-1]
# Define labels based on sorted clusters
labels = ["Cheater", "Good Player", "Average Player", "Poor Player"]
label_mapping = {cluster: labels[i] for i, cluster in enumerate(sorted_clusters)}

# Assign labels to the original data
data["cluster"] = kmeans.labels_
data["category"] = data["cluster"].map(label_mapping)

# Function to predict category for new data
def predict_category(new_illegal_traces, new_trigger_time, new_flick_speed):
    # Preprocess new data
    new_trigger_time = 1 / max(new_trigger_time, epsilon) 
    if (new_trigger_time > 1):
        new_trigger_time = 1
    cheating_score = (new_illegal_traces / 50) + (new_trigger_time * 100) + (new_flick_speed / 100)
    X_new = np.array([[cheating_score]])
    X_new_scaled = scaler.transform(X_new)
    cluster = kmeans.predict(X_new_scaled)[0]
    return label_mapping[cluster]

# Metrics from clustered data
silhouette = silhouette_score(X_scaled, data['cluster'])
davies = davies_bouldin_score(X_scaled, data['cluster'])
calinski = calinski_harabasz_score(X_scaled, data['cluster'])

print(f"Silhoutte Score: {silhouette:.3f} (Closer to 1 = better)")
print(f"Davies-Bouldin Index: {davies:.3f} (closer to 0 = better)")
print(f"Calinski-Harabasaz: {calinski:.3f} (Higher = better)")

#Feature contribution analysis
print(data.groupby("category")["cheating_score"].describe())

# Example usage:
"""
print(predict_category(20, 100, 300))
print(predict_category(60, 30, 500))
print(predict_category(731, 525.1, 700))
print(predict_category(400, 0.1, 800))
"""
print(predict_category(214	,35,	345.8168))
print(predict_category(428,	46,	432.9412))
print(predict_category(26	,45,	317.1127))
print(predict_category(84	,2	,399.9944))
print(predict_category(36	,6	,389.1838))
print(predict_category(62	,128,	392.7874))
print(predict_category(2	,0	,374.7696))
print(predict_category(18	,11,	378.3731))
print(predict_category(0	,0,	165.7635))
print(predict_category(0	,0,	392.7874))
print(predict_category(2	,0,	353.1483))
print(predict_category(0	,0,	335.1305))




# visualizing data
# Get centroids in original scale
centroids_original = scaler.inverse_transform(kmeans.cluster_centers_.reshape(-1, 1)).flatten()
"""
# Plot histogram
plt.figure(figsize=(10, 6))
n, bins, patches = plt.hist(data['cheating_score'], bins=30, color='skyblue', edgecolor='black', alpha=0.7)

# Add centroids as vertical lines
colors = ['red', 'orange', 'green', 'blue']
labels = ['Cheater', 'Good Player', 'Average', 'Poor Player']
for centroid, color, label in zip(centroids_original, colors, labels):
    plt.axvline(centroid, color=color, linestyle='--', linewidth=2, label=f'{label} Centroid')

plt.title('Cheating Score Distribution with Cluster Boundaries', pad=20)
plt.xlabel('Cheating Score')
plt.ylabel('Frequency')
plt.legend()
plt.grid(axis='y', alpha=0.3)
plt.show()

# Prepare data for boxplot
box_data = [data[data['category'] == cat]['cheating_score'] for cat in labels]

plt.figure(figsize=(10, 6))
box = plt.boxplot(
    box_data,
    patch_artist=True,
    labels=labels,
    medianprops={'color': 'black', 'linewidth': 2}
)

# Color boxes
for patch, color in zip(box['boxes'], colors):
    patch.set_facecolor(color)

plt.title('Cheating Score Distribution by Cluster', pad=20)
plt.xlabel('Category')
plt.ylabel('Cheating Score')
plt.xticks(rotation=45)
plt.grid(axis='y', alpha=0.3)
plt.show()

# Compute centroids in original scale
centroids_original = scaler.inverse_transform(kmeans.cluster_centers_.reshape(-1, 1)).flatten()

# Assign colors to clusters

# Create a scatter plot with jittered y-values for visibility
plt.figure(figsize=(12, 6))

# Plot data points with jitter
np.random.seed(42)  # For reproducibility
y_jitter = np.random.normal(0, 0.05, size=len(data))  # Small random noise for y-axis

plt.scatter(
    data['cheating_score'],
    y_jitter,
    c="blue",
    alpha=0.6,
    s=50,
    edgecolor='red',
    linewidth=0.5
)   
i = 0
labels = ['Poor Player', 'Good Player', 'Cheater', 'Average Player']
# Add centroids as vertical lines
for centroid, color, label in zip(centroids_original, colors, labels):

    plt.axvline(
        centroid,
        color=colors[i],
        linestyle='--',
        linewidth=2,
        label= labels[i] + ' Centroid'
    )
    i = i + 1

# Customize plot
plt.title('Cheating Score Clusters (1D Scatter Plot)', pad=20)
plt.xlabel('Cheating Score')
plt.yticks([])  # Hide dummy y-axis
plt.legend(bbox_to_anchor=(1.05, 1), loc='upper left')
plt.grid(axis='x', alpha=0.3)
plt.show()
"""


unique_ids = data['dataset_type'].unique()
num_ids = len(unique_ids)
cmap = ListedColormap(plt.get_cmap("tab20")(np.linspace(0, 1, num_ids)))  # Use 'tab20' for up to 20 colors
id_to_color = {id: cmap(i) for i, id in enumerate(unique_ids)}
data['dataset_color'] = data['dataset_type'].map(id_to_color)

plt.figure(figsize=(12, 6))
np.random.seed(42)  # For consistent jitter
y_jitter = data['illegal_traces']

# Plot points colored by dataset_id
plt.scatter(
    data['cheating_score'],
    y_jitter,
    c=data['dataset_color'],  # Color by dataset identifier
    alpha=0.6,
    s=50,
    edgecolor='k',
    linewidth=0.5,
    label='Data Points'
)

# Add centroids (from previous clustering)
centroids_original = scaler.inverse_transform(kmeans.cluster_centers_.reshape(-1, 1)).flatten()
colors = ['red', 'orange', 'green', 'blue']  # Cluster centroid colors
labels = ['Poor Player', 'Good Player', 'Cheater', 'Average']

for centroid, color, label in zip(centroids_original, colors, labels):
    plt.axvline(
        centroid,
        color=color,
        linestyle='--',
        linewidth=2,
        label=f'{label} Centroid'
    )

# Add legends
legend_elements = [
    plt.Line2D([0], [0], marker='o', color='w', label=id,
              markersize=10, markerfacecolor=id_to_color[id])
    for id in unique_ids
] + [
    plt.Line2D([0], [0], color=color, linestyle='--', label=f'{label} Centroid')
    for color, label in zip(colors, labels)
]

plt.legend(
    handles=legend_elements,
    bbox_to_anchor=(1.05, 1),
    loc='upper left',
    title='Legend'
)

plt.title('Cheating Score Clusters by Illegal Trace Time', pad=20)
plt.xlabel('Cheating Score')
plt.ylabel('Illegal Trace Time (ms)')
plt.grid(axis='x', alpha=0.3)
plt.show()

plt.figure(figsize=(12, 6))
np.random.seed(42)  # For consistent jitter
y_jitter = data['trigger_time']

# Plot points colored by dataset_id
plt.scatter(
    data['cheating_score'],
    y_jitter,
    c=data['dataset_color'],  # Color by dataset identifier
    alpha=0.6,
    s=50,
    edgecolor='k',
    linewidth=0.5,
    label='Data Points'
)

# Add centroids (from previous clustering)
centroids_original = scaler.inverse_transform(kmeans.cluster_centers_.reshape(-1, 1)).flatten()
colors = ['red', 'orange', 'green', 'blue']  # Cluster centroid colors
labels = ['Poor Player', 'Good Player', 'Cheater', 'Average']

for centroid, color, label in zip(centroids_original, colors, labels):
    plt.axvline(
        centroid,
        color=color,
        linestyle='--',
        linewidth=2,
        label=f'{label} Centroid'
    )

# Add legends
legend_elements = [
    plt.Line2D([0], [0], marker='o', color='w', label=id,
              markersize=10, markerfacecolor=id_to_color[id])
    for id in unique_ids
] + [
    plt.Line2D([0], [0], color=color, linestyle='--', label=f'{label} Centroid')
    for color, label in zip(colors, labels)
]

plt.legend(
    handles=legend_elements,
    bbox_to_anchor=(1.05, 1),
    loc='upper left',
    title='Legend'
)

plt.title('Cheating Score Clusters by Trigger Time', pad=20)
plt.xlabel('Cheating Score')
plt.ylabel('Trigger Time (ms)')
plt.grid(axis='x', alpha=0.3)
plt.show()

plt.figure(figsize=(12, 6))
np.random.seed(42)  # For consistent jitter
y_jitter = data['flick_speed']

# Plot points colored by dataset_id
plt.scatter(
    data['cheating_score'],
    y_jitter,
    c=data['dataset_color'],  # Color by dataset identifier
    alpha=0.6,
    s=50,
    edgecolor='k',
    linewidth=0.5,
    label='Data Points'
)

# Add centroids (from previous clustering)
centroids_original = scaler.inverse_transform(kmeans.cluster_centers_.reshape(-1, 1)).flatten()
colors = ['red', 'orange', 'green', 'blue']  # Cluster centroid colors
labels = ['Poor Player', 'Good Player', 'Cheater', 'Average']

for centroid, color, label in zip(centroids_original, colors, labels):
    plt.axvline(
        centroid,
        color=color,
        linestyle='--',
        linewidth=2,
        label=f'{label} Centroid'
    )

# Add legends
legend_elements = [
    plt.Line2D([0], [0], marker='o', color='w', label=id,
              markersize=10, markerfacecolor=id_to_color[id])
    for id in unique_ids
] + [
    plt.Line2D([0], [0], color=color, linestyle='--', label=f'{label} Centroid')
    for color, label in zip(colors, labels)
]
    
plt.legend(
    handles=legend_elements,
    bbox_to_anchor=(1.05, 1),
    loc='upper left',
    title='Legend'
)

plt.title('Cheating Score Clusters by Flick Speed', pad=20)
plt.xlabel('Cheating Score')
plt.ylabel('Flick Speed (pixels/ms)')
plt.grid(axis='x', alpha=0.3)
plt.show()