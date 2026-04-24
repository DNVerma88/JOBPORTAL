-- ============================================================
-- Seed: master.Skills (60+ skills with NormalizedName + Slug)
-- UUID prefix: 00000007-0000-0000-0000-000000000NNN
-- ============================================================

INSERT INTO master."Skills" ("Id", "Name", "NormalizedName", "Slug", "Category", "UsageCount")
VALUES
    -- ── Languages ──────────────────────────────────────────────
    ('00000007-0000-0000-0000-000000000001', 'JavaScript',       'javascript',       'javascript',        'Language',    0),
    ('00000007-0000-0000-0000-000000000002', 'TypeScript',       'typescript',       'typescript',        'Language',    0),
    ('00000007-0000-0000-0000-000000000003', 'Python',           'python',           'python',            'Language',    0),
    ('00000007-0000-0000-0000-000000000004', 'Java',             'java',             'java',              'Language',    0),
    ('00000007-0000-0000-0000-000000000005', 'C#',               'c#',               'csharp',            'Language',    0),
    ('00000007-0000-0000-0000-000000000006', 'Go',               'go',               'go',                'Language',    0),
    ('00000007-0000-0000-0000-000000000007', 'Rust',             'rust',             'rust',              'Language',    0),
    ('00000007-0000-0000-0000-000000000008', 'Kotlin',           'kotlin',           'kotlin',            'Language',    0),
    ('00000007-0000-0000-0000-000000000009', 'Swift',            'swift',            'swift',             'Language',    0),
    ('00000007-0000-0000-0000-000000000010', 'PHP',              'php',              'php',               'Language',    0),
    ('00000007-0000-0000-0000-000000000011', 'Ruby',             'ruby',             'ruby',              'Language',    0),
    ('00000007-0000-0000-0000-000000000012', 'C++',              'c++',              'cpp',               'Language',    0),
    ('00000007-0000-0000-0000-000000000013', 'Scala',            'scala',            'scala',             'Language',    0),
    ('00000007-0000-0000-0000-000000000014', 'R',                'r',                'r-lang',            'Language',    0),

    -- ── Frontend Frameworks ────────────────────────────────────
    ('00000007-0000-0000-0000-000000000015', 'React',            'react',            'react',             'Frontend',    0),
    ('00000007-0000-0000-0000-000000000016', 'Angular',          'angular',          'angular',           'Frontend',    0),
    ('00000007-0000-0000-0000-000000000017', 'Vue.js',           'vue.js',           'vuejs',             'Frontend',    0),
    ('00000007-0000-0000-0000-000000000018', 'Next.js',          'next.js',          'nextjs',            'Frontend',    0),
    ('00000007-0000-0000-0000-000000000019', 'Nuxt.js',          'nuxt.js',          'nuxtjs',            'Frontend',    0),
    ('00000007-0000-0000-0000-000000000020', 'Svelte',           'svelte',           'svelte',            'Frontend',    0),

    -- ── Backend Frameworks ─────────────────────────────────────
    ('00000007-0000-0000-0000-000000000021', 'Node.js',          'node.js',          'nodejs',            'Backend',     0),
    ('00000007-0000-0000-0000-000000000022', '.NET',             '.net',             'dotnet',            'Backend',     0),
    ('00000007-0000-0000-0000-000000000023', 'Spring Boot',      'spring boot',      'spring-boot',       'Backend',     0),
    ('00000007-0000-0000-0000-000000000024', 'Django',           'django',           'django',            'Backend',     0),
    ('00000007-0000-0000-0000-000000000025', 'FastAPI',          'fastapi',          'fastapi',           'Backend',     0),
    ('00000007-0000-0000-0000-000000000026', 'Flask',            'flask',            'flask',             'Backend',     0),
    ('00000007-0000-0000-0000-000000000027', 'Express.js',       'express.js',       'expressjs',         'Backend',     0),
    ('00000007-0000-0000-0000-000000000028', 'Laravel',          'laravel',          'laravel',           'Backend',     0),
    ('00000007-0000-0000-0000-000000000029', 'Ruby on Rails',    'ruby on rails',    'ruby-on-rails',     'Backend',     0),

    -- ── Databases ──────────────────────────────────────────────
    ('00000007-0000-0000-0000-000000000030', 'PostgreSQL',       'postgresql',       'postgresql',        'Database',    0),
    ('00000007-0000-0000-0000-000000000031', 'MySQL',            'mysql',            'mysql',             'Database',    0),
    ('00000007-0000-0000-0000-000000000032', 'MongoDB',          'mongodb',          'mongodb',           'Database',    0),
    ('00000007-0000-0000-0000-000000000033', 'Redis',            'redis',            'redis',             'Database',    0),
    ('00000007-0000-0000-0000-000000000034', 'Elasticsearch',    'elasticsearch',    'elasticsearch',     'Database',    0),
    ('00000007-0000-0000-0000-000000000035', 'Microsoft SQL Server', 'microsoft sql server', 'mssql',     'Database',    0),
    ('00000007-0000-0000-0000-000000000036', 'Cassandra',        'cassandra',        'cassandra',         'Database',    0),
    ('00000007-0000-0000-0000-000000000037', 'DynamoDB',         'dynamodb',         'dynamodb',          'Database',    0),

    -- ── Cloud & DevOps ─────────────────────────────────────────
    ('00000007-0000-0000-0000-000000000038', 'AWS',              'aws',              'aws',               'Cloud',       0),
    ('00000007-0000-0000-0000-000000000039', 'Azure',            'azure',            'azure',             'Cloud',       0),
    ('00000007-0000-0000-0000-000000000040', 'GCP',              'gcp',              'gcp',               'Cloud',       0),
    ('00000007-0000-0000-0000-000000000041', 'Docker',           'docker',           'docker',            'DevOps',      0),
    ('00000007-0000-0000-0000-000000000042', 'Kubernetes',       'kubernetes',       'kubernetes',        'DevOps',      0),
    ('00000007-0000-0000-0000-000000000043', 'Terraform',        'terraform',        'terraform',         'DevOps',      0),
    ('00000007-0000-0000-0000-000000000044', 'Ansible',          'ansible',          'ansible',           'DevOps',      0),
    ('00000007-0000-0000-0000-000000000045', 'Jenkins',          'jenkins',          'jenkins',           'DevOps',      0),
    ('00000007-0000-0000-0000-000000000046', 'GitHub Actions',   'github actions',   'github-actions',    'DevOps',      0),
    ('00000007-0000-0000-0000-000000000047', 'GitLab CI/CD',     'gitlab ci/cd',     'gitlab-cicd',       'DevOps',      0),

    -- ── Mobile ─────────────────────────────────────────────────
    ('00000007-0000-0000-0000-000000000048', 'Flutter',          'flutter',          'flutter',           'Mobile',      0),
    ('00000007-0000-0000-0000-000000000049', 'React Native',     'react native',     'react-native',      'Mobile',      0),
    ('00000007-0000-0000-0000-000000000050', 'Android SDK',      'android sdk',      'android-sdk',       'Mobile',      0),
    ('00000007-0000-0000-0000-000000000051', 'iOS (UIKit/SwiftUI)', 'ios (uikit/swiftui)', 'ios-swiftui', 'Mobile',      0),

    -- ── Data & AI ──────────────────────────────────────────────
    ('00000007-0000-0000-0000-000000000052', 'TensorFlow',       'tensorflow',       'tensorflow',        'AI/ML',       0),
    ('00000007-0000-0000-0000-000000000053', 'PyTorch',          'pytorch',          'pytorch',           'AI/ML',       0),
    ('00000007-0000-0000-0000-000000000054', 'Scikit-Learn',     'scikit-learn',     'scikit-learn',      'AI/ML',       0),
    ('00000007-0000-0000-0000-000000000055', 'Pandas',           'pandas',           'pandas',            'Data',        0),
    ('00000007-0000-0000-0000-000000000056', 'Apache Spark',     'apache spark',     'apache-spark',      'Data',        0),
    ('00000007-0000-0000-0000-000000000057', 'Kafka',            'kafka',            'kafka',             'Data',        0),

    -- ── Design ─────────────────────────────────────────────────
    ('00000007-0000-0000-0000-000000000058', 'Figma',            'figma',            'figma',             'Design',      0),
    ('00000007-0000-0000-0000-000000000059', 'Adobe XD',         'adobe xd',         'adobe-xd',          'Design',      0),
    ('00000007-0000-0000-0000-000000000060', 'Sketch',           'sketch',           'sketch',            'Design',      0),

    -- ── Other ──────────────────────────────────────────────────
    ('00000007-0000-0000-0000-000000000061', 'GraphQL',          'graphql',          'graphql',           'API',         0),
    ('00000007-0000-0000-0000-000000000062', 'REST APIs',        'rest apis',        'rest-apis',         'API',         0),
    ('00000007-0000-0000-0000-000000000063', 'gRPC',             'grpc',             'grpc',              'API',         0),
    ('00000007-0000-0000-0000-000000000064', 'Microservices',    'microservices',    'microservices',     'Architecture', 0),
    ('00000007-0000-0000-0000-000000000065', 'Git',              'git',              'git',               'Tool',        0),
    ('00000007-0000-0000-0000-000000000066', 'Linux',            'linux',            'linux',             'Tool',        0),
    ('00000007-0000-0000-0000-000000000067', 'Agile / Scrum',    'agile / scrum',    'agile-scrum',       'Methodology', 0)
ON CONFLICT DO NOTHING;
