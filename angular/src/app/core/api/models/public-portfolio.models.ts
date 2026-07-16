export interface ProjectPublicListDto {
  id: string;
  name: string;
  slug: string;
  summary: string;
  thumbnailUrl: string;
  isFeatured: boolean;
  displayOrder: number;
}

export interface SkillPublicDto {
  name: string;
  iconUrl: string | null;
  levelLabel: string | null;
}

export interface SkillGroupPublicDto {
  category: string;
  items: SkillPublicDto[];
}

export interface ProfilePublicDto {
  displayName: string;
  headline: string | null;
  bio: string | null;
  avatarUrl: string | null;
  cvUrl: string | null;
  email: string | null;
  socialLinks: string[];
}

export interface PublicPortfolioDto {
  profile: ProfilePublicDto | null;
  featuredProjects: ProjectPublicListDto[];
  skillGroups: SkillGroupPublicDto[];
}
