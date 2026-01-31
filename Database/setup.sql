-- CÓDIGO SQL PARA SER EXECUTADO PARA CRIAÇÃO DO BANCO DE DADOS

create table Users(
  id uuid default gen_random_uuid(),
  name varchar(255) not null,
  email varchar(255) not null,
  role varchar(20) default 'USER',
  hashPassword text not null,
  resetPasswordToken text,
  bio text,
  avatar text,
  isVerified boolean default false,
  verificationToken text,
  --followersCount int default 0,
  --followingCount int default 0,
  createdAt timestamp with time zone default now(),
  updatedAt timestamp with time zone default now(),
  deletedAt timestamp with time zone,
  primary key (id),
  unique (email),
  constraint email_format_chk check(email ~* '^[A-Za-z0-9._%-]+@[A-Za-z0-9.-]+[.][A-Za-z]+$'),
  constraint role_type_chk check(role in ('USER', 'ADMIN'))
);

create table Users_Followers(
  followedId uuid not null,
  followerId uuid not null,
  primary key (followedId, followerId),
  foreign key (followedId) references Users(id),
  foreign key (followerId) references Users(id)
);

create table Artists(
  id uuid default gen_random_uuid(),
  name varchar(255) not null,
  bio text,
  avatar text,
  --followersCount int default 0,
  --postsCount int default 0,
  createdAt timestamp with time zone default now(),
  updatedAt timestamp with time zone default now(),
  deletedAt timestamp with time zone,
  primary key (id),
  unique (name)
);

create table Artists_Followers(
  artistId uuid not null,
  userId uuid not null,
  primary key (artistId, userId),
  foreign key (artistId) references Artists(id),
  foreign key (userId) references Users(id)
);

create table Posts(
  id uuid default gen_random_uuid(),
  title varchar(255) not null,
  content text not null,
  authorId uuid not null,
  artistId uuid not null,
  --likesCount int default 0,
  --dislikesCount int default 0,
  createdAt timestamp with time zone default now(),
  updatedAt timestamp with time zone default now(),
  deletedAt timestamp with time zone,
  primary key (id),
  foreign key (authorId) references Users(id),
  foreign key (artistId) references Artists(id)
);

create table Post_Reactions(
  postId uuid not null,
  userId uuid not null,
  reactionType varchar(20) not null,
  primary key (postId, userId),
  foreign key (postId) references Posts(id),
  foreign key (userId) references Users(id),
  constraint reaction_type_chk check(reactionType in ('LIKE', 'DISLIKE'))
);

create table Replies(
  id uuid default gen_random_uuid(),
  content text not null,
  authorId uuid not null,
  postId uuid not null,
  parentId uuid,
  --likesCount int default 0,
  --dislikesCount int default 0,
  createdAt timestamp with time zone default now(),
  updatedAt timestamp with time zone default now(),
  deletedAt timestamp with time zone,
  primary key (id),
  foreign key (authorId) references Users(id),
  foreign key (postId) references Posts(id),
  foreign key (parentId) references Replies(id)
);

create table Replies_Reactions(
  replyId uuid not null,
  userId uuid not null,
  reactionType varchar(20) not null,
  primary key (replyId, userId),
  foreign key (replyId) references Replies(id),
  foreign key (userId) references Users(id),
  constraint reaction_type_chk check(reactionType in ('LIKE', 'DISLIKE'))
);

-- ÍNDICES PARA MELHORAR PERFORMANCE
create index idx_posts_author on Posts(authorId);
create index idx_reactions_user on Post_Reactions(userId);

create index idx_replies_post on Replies(postId);
create index idx_replies_parent on Replies(parentId);
create index idx_replies_reactions_user on Replies_Reactions(userId);

create index idx_active_posts on Posts(artistId) where deletedAt is null;